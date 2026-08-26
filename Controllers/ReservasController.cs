using System.Globalization;
using BarberFlow.Data;
using BarberFlow.Enums;
using BarberFlow.Interfaces;
using BarberFlow.Models;
using BarberFlow.ViewModels.Reservas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

[Authorize(Roles = "Cliente")]
public class ReservasController : Controller
{
    private readonly BarberFlowDbContext _context;
    private readonly IDisponibilidadeService _disponibilidade;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(
        BarberFlowDbContext context,
        IDisponibilidadeService disponibilidade,
        ILogger<ReservasController> logger)
    {
        _context = context;
        _disponibilidade = disponibilidade;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var email = User.Identity?.Name;
        var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
        if (cliente is null) return RedirectToAction("Registro", "Conta");

        var reservas = await _context.Reservas.AsNoTracking()
            .Include(r => r.Servico).Include(r => r.Profissional).Include(r => r.Avaliacao)
            .Where(r => r.ClienteId == cliente.Id)
            .OrderByDescending(r => r.DataHoraInicio)
            .ToListAsync();
        return View(reservas);
    }

    [HttpGet]
    public async Task<IActionResult> Nova(int? servicoId, int? profissionalId, DateTime? data)
    {
        var model = new NovaReservaViewModel
        {
            ServicoId = servicoId ?? 0,
            ProfissionalId = profissionalId ?? 0,
            Data = (data ?? DateTime.Today.AddDays(1)).Date
        };
        await PrepararAsync(model);
        if (model.ServicoId > 0 && model.ProfissionalId == 0 && model.Profissionais.Count == 1)
            model.ProfissionalId = int.Parse(model.Profissionais[0].Value!);
        if (model.ServicoId > 0 && model.ProfissionalId > 0)
            model.HorariosDisponiveis = await _disponibilidade.ObterHorariosAsync(model.ProfissionalId, model.ServicoId, model.Data);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nova(NovaReservaViewModel model)
    {
        if (model.Data.Date < DateTime.Today)
            ModelState.AddModelError(nameof(model.Data), "Escolha uma data de hoje em diante.");

        if (!TimeSpan.TryParseExact(model.Horario, @"hh\:mm", CultureInfo.InvariantCulture, out var hora))
            ModelState.AddModelError(nameof(model.Horario), "Selecione um horário válido.");

        var inicio = model.Data.Date.Add(hora);
        if (ModelState.IsValid && !await _disponibilidade.HorarioEstaDisponivelAsync(model.ProfissionalId, model.ServicoId, inicio))
            ModelState.AddModelError(nameof(model.Horario), "Esse horário acabou de ficar indisponível. Escolha outro.");

        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity!.Name);
        var servico = await _context.Servicos.FindAsync(model.ServicoId);
        if (cliente is null || servico is null)
            ModelState.AddModelError(string.Empty, "Não foi possível identificar o cliente ou o serviço.");

        if (!ModelState.IsValid)
        {
            await PrepararAsync(model);
            if (model.ServicoId > 0 && model.ProfissionalId > 0)
                model.HorariosDisponiveis = await _disponibilidade.ObterHorariosAsync(model.ProfissionalId, model.ServicoId, model.Data);
            return View(model);
        }

        // A disponibilidade é conferida novamente imediatamente antes da gravação.
        if (!await _disponibilidade.HorarioEstaDisponivelAsync(model.ProfissionalId, model.ServicoId, inicio))
        {
            ModelState.AddModelError(nameof(model.Horario), "Esse horário não está mais disponível.");
            await PrepararAsync(model);
            model.HorariosDisponiveis = await _disponibilidade.ObterHorariosAsync(model.ProfissionalId, model.ServicoId, model.Data);
            return View(model);
        }

        _context.Reservas.Add(new Reserva
        {
            ClienteId = cliente!.Id,
            ProfissionalId = model.ProfissionalId,
            ServicoId = model.ServicoId,
            DataHoraInicio = inicio,
            DataHoraFim = inicio.AddMinutes(servico!.DuracaoMinutos),
            Status = StatusReserva.Confirmada
        });
        await _context.SaveChangesAsync();
        _logger.LogInformation("Reserva criada para o cliente {ClienteId}, profissional {ProfissionalId}, serviço {ServicoId}, início {Inicio}", cliente.Id, model.ProfissionalId, model.ServicoId, inicio);
        TempData["Sucesso"] = "Reserva confirmada com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var email = User.Identity?.Name;
        var reserva = await _context.Reservas.Include(r => r.Cliente)
            .FirstOrDefaultAsync(r => r.Id == id && r.Cliente.Email == email);
        if (reserva is null) return NotFound();
        if (reserva.DataHoraInicio <= DateTime.Now)
        {
            TempData["Erro"] = "Não é possível cancelar uma reserva que já começou.";
            return RedirectToAction(nameof(Index));
        }
        if (reserva.Status is StatusReserva.Cancelada or StatusReserva.Concluida)
        {
            TempData["Erro"] = "Esta reserva não pode mais ser cancelada.";
            return RedirectToAction(nameof(Index));
        }
        reserva.Status = StatusReserva.Cancelada;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Reserva {ReservaId} cancelada pelo cliente {ClienteId}", reserva.Id, reserva.ClienteId);
        TempData["Sucesso"] = "Reserva cancelada. O horário voltou a ficar disponível.";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Reagendar(int id, DateTime? data)
    {
        var email = User.Identity?.Name;
        var reserva = await _context.Reservas.AsNoTracking().Include(r => r.Cliente).Include(r => r.Servico).Include(r => r.Profissional)
            .FirstOrDefaultAsync(r => r.Id == id && r.Cliente.Email == email);
        if (reserva is null) return NotFound();
        if (reserva.Status is StatusReserva.Cancelada or StatusReserva.Concluida || reserva.DataHoraInicio <= DateTime.Now)
        {
            TempData["Erro"] = "Esta reserva não pode ser reagendada.";
            return RedirectToAction(nameof(Index));
        }

        var model = new ReagendarReservaViewModel
        {
            ReservaId = reserva.Id, ServicoNome = reserva.Servico.Nome, ProfissionalNome = reserva.Profissional.Nome,
            Data = (data ?? reserva.DataHoraInicio).Date
        };
        model.HorariosDisponiveis = (await _disponibilidade.ObterHorariosAsync(reserva.ProfissionalId, reserva.ServicoId, model.Data)).ToList();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reagendar(ReagendarReservaViewModel model)
    {
        var email = User.Identity?.Name;
        var reserva = await _context.Reservas.Include(r => r.Cliente).Include(r => r.Servico).Include(r => r.Profissional)
            .FirstOrDefaultAsync(r => r.Id == model.ReservaId && r.Cliente.Email == email);
        if (reserva is null) return NotFound();

        model.ServicoNome = reserva.Servico.Nome;
        model.ProfissionalNome = reserva.Profissional.Nome;
        if (reserva.Status is StatusReserva.Cancelada or StatusReserva.Concluida || reserva.DataHoraInicio <= DateTime.Now)
            ModelState.AddModelError(string.Empty, "Esta reserva não pode ser reagendada.");

        if (!TimeSpan.TryParseExact(model.Horario, @"hh\:mm", CultureInfo.InvariantCulture, out var hora))
            ModelState.AddModelError(nameof(model.Horario), "Selecione um horário válido.");
        var novoInicio = model.Data.Date.Add(hora);

        if (ModelState.IsValid && novoInicio != reserva.DataHoraInicio &&
            !await _disponibilidade.HorarioEstaDisponivelAsync(reserva.ProfissionalId, reserva.ServicoId, novoInicio))
            ModelState.AddModelError(nameof(model.Horario), "Esse horário não está mais disponível.");

        if (!ModelState.IsValid)
        {
            model.HorariosDisponiveis = (await _disponibilidade.ObterHorariosAsync(reserva.ProfissionalId, reserva.ServicoId, model.Data)).ToList();
            return View(model);
        }

        reserva.DataHoraInicio = novoInicio;
        reserva.DataHoraFim = novoInicio.AddMinutes(reserva.Servico.DuracaoMinutos);
        reserva.Status = StatusReserva.Confirmada;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Reserva {ReservaId} reagendada para {NovoInicio}", reserva.Id, novoInicio);
        TempData["Sucesso"] = "Reserva reagendada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PrepararAsync(NovaReservaViewModel model)
    {
        model.Servicos = await _context.Servicos.AsNoTracking().Where(s => s.Ativo)
            .OrderBy(s => s.Nome).Select(s => new SelectListItem(s.Nome, s.Id.ToString())).ToListAsync();
        var profissionais = _context.Profissionais.AsNoTracking().Where(p => p.Ativo);
        if (model.ServicoId > 0)
            profissionais = profissionais.Where(p => p.ProfissionalServicos.Any(ps => ps.ServicoId == model.ServicoId));
        model.Profissionais = await profissionais.OrderBy(p => p.Nome)
            .Select(p => new SelectListItem(p.Nome, p.Id.ToString())).ToListAsync();
    }
}
