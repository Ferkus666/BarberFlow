using BarberFlow.Data;
using BarberFlow.Enums;
using BarberFlow.Models;
using BarberFlow.ViewModels.Avaliacoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

[Authorize(Roles = "Cliente")]
public class AvaliacoesController : Controller
{
    private readonly BarberFlowDbContext _context;

    public AvaliacoesController(BarberFlowDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Criar(int reservaId)
    {
        var reserva = await BuscarReservaDoClienteAsync(reservaId);
        if (reserva is null) return NotFound();
        if (reserva.Status != StatusReserva.Concluida)
        {
            TempData["Erro"] = "A avaliação fica disponível após a conclusão do atendimento.";
            return RedirectToAction("Index", "Reservas");
        }
        if (await _context.Avaliacoes.AnyAsync(a => a.ReservaId == reserva.Id))
        {
            TempData["Erro"] = "Este atendimento já foi avaliado.";
            return RedirectToAction("Index", "Reservas");
        }
        return View(new AvaliacaoFormViewModel { ReservaId = reserva.Id, ProfissionalNome = reserva.Profissional.Nome, ServicoNome = reserva.Servico.Nome });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(AvaliacaoFormViewModel model)
    {
        var reserva = await BuscarReservaDoClienteAsync(model.ReservaId);
        if (reserva is null) return NotFound();
        model.ProfissionalNome = reserva.Profissional.Nome;
        model.ServicoNome = reserva.Servico.Nome;
        if (reserva.Status != StatusReserva.Concluida)
            ModelState.AddModelError(string.Empty, "Só é possível avaliar atendimentos concluídos.");
        if (await _context.Avaliacoes.AnyAsync(a => a.ReservaId == reserva.Id))
            ModelState.AddModelError(string.Empty, "Este atendimento já foi avaliado.");
        if (!ModelState.IsValid) return View(model);

        _context.Avaliacoes.Add(new Avaliacao
        {
            ReservaId = reserva.Id,
            ClienteId = reserva.ClienteId,
            ProfissionalId = reserva.ProfissionalId,
            Nota = model.Nota,
            Comentario = model.Comentario?.Trim()
        });
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Obrigado! Sua avaliação foi publicada.";
        return RedirectToAction("Detalhes", "Profissionais", new { id = reserva.ProfissionalId });
    }

    private async Task<Reserva?> BuscarReservaDoClienteAsync(int id)
    {
        var email = User.Identity?.Name;
        return await _context.Reservas
            .Include(r => r.Cliente).Include(r => r.Profissional).Include(r => r.Servico)
            .FirstOrDefaultAsync(r => r.Id == id && r.Cliente.Email == email);
    }
}
