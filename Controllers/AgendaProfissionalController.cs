using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberFlow.Controllers;

[Authorize(Roles = IdentitySeed.ProfissionalRole)]
public class AgendaProfissionalController : Controller
{
    private readonly BarberFlowDbContext _context;
    public AgendaProfissionalController(BarberFlowDbContext context) => _context = context;

    public async Task<IActionResult> Index(DateTime? data)
    {
        var dia = (data ?? DateTime.Today).Date;
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profissional = await _context.Profissionais.AsNoTracking().FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        if (profissional is null) return RedirectToAction("AcessoNegado", "Conta");

        var reservas = await _context.Reservas.AsNoTracking()
            .Include(r => r.Cliente).Include(r => r.Servico)
            .Where(r => r.ProfissionalId == profissional.Id && r.DataHoraInicio >= dia && r.DataHoraInicio < dia.AddDays(1))
            .OrderBy(r => r.DataHoraInicio).ToListAsync();

        ViewBag.Data = dia;
        ViewBag.Total = reservas.Count;
        ViewBag.Pendentes = reservas.Count(r => r.Status == StatusReserva.Pendente);
        ViewBag.Confirmadas = reservas.Count(r => r.Status == StatusReserva.Confirmada);
        ViewBag.Concluidas = reservas.Count(r => r.Status == StatusReserva.Concluida);
        ViewBag.ProximoId = reservas.FirstOrDefault(r => r.DataHoraInicio >= DateTime.Now && r.Status is StatusReserva.Pendente or StatusReserva.Confirmada)?.Id;
        return View(reservas);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(int id, StatusReserva status, DateTime? data)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reserva = await _context.Reservas.Include(r => r.Profissional)
            .FirstOrDefaultAsync(r => r.Id == id && r.Profissional.UsuarioId == usuarioId);
        if (reserva is null) return NotFound();

        var permitidos = new[] { StatusReserva.Confirmada, StatusReserva.Concluida, StatusReserva.Cancelada, StatusReserva.NaoCompareceu };
        if (!permitidos.Contains(status)) return BadRequest();
        if (reserva.Status is StatusReserva.Cancelada or StatusReserva.Concluida)
        {
            TempData["Erro"] = "Esta reserva já está encerrada e não pode ser alterada.";
            return RedirectToAction(nameof(Index), new { data });
        }
        if (status == StatusReserva.Concluida && reserva.DataHoraInicio > DateTime.Now)
        {
            TempData["Erro"] = "Um atendimento futuro não pode ser marcado como concluído.";
            return RedirectToAction(nameof(Index), new { data });
        }

        reserva.Status = status;
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = $"Status alterado para {status}.";
        return RedirectToAction(nameof(Index), new { data });
    }
}
