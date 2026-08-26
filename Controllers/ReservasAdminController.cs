using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Enums;
using BarberFlow.ViewModels.Reservas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

[Authorize(Roles = IdentitySeed.AdministradorRole)]
public class ReservasAdminController : Controller
{
    private readonly BarberFlowDbContext _context;
    public ReservasAdminController(BarberFlowDbContext context) => _context = context;

    public async Task<IActionResult> Index(ReservaFiltroViewModel filtro)
    {
        var query = _context.Reservas.AsNoTracking().Include(r => r.Cliente).Include(r => r.Profissional).Include(r => r.Servico).AsQueryable();
        if (filtro.Data.HasValue)
        {
            var inicio = filtro.Data.Value.Date;
            query = query.Where(r => r.DataHoraInicio >= inicio && r.DataHoraInicio < inicio.AddDays(1));
        }
        if (filtro.ProfissionalId.HasValue) query = query.Where(r => r.ProfissionalId == filtro.ProfissionalId);
        if (filtro.ServicoId.HasValue) query = query.Where(r => r.ServicoId == filtro.ServicoId);
        if (filtro.Status.HasValue) query = query.Where(r => r.Status == filtro.Status);

        filtro.Reservas = await query.OrderByDescending(r => r.DataHoraInicio).ToListAsync();
        filtro.Profissionais = await _context.Profissionais.AsNoTracking().OrderBy(p => p.Nome).Select(p => new SelectListItem(p.Nome, p.Id.ToString())).ToListAsync();
        filtro.Servicos = await _context.Servicos.AsNoTracking().OrderBy(s => s.Nome).Select(s => new SelectListItem(s.Nome, s.Id.ToString())).ToListAsync();
        return View(filtro);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(int id, StatusReserva status)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva is null) return NotFound();
        if (reserva.Status is StatusReserva.Cancelada or StatusReserva.Concluida)
        {
            TempData["Erro"] = "Reservas encerradas não podem ter o status alterado.";
            return RedirectToAction(nameof(Index));
        }
        reserva.Status = status;
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Status da reserva atualizado.";
        return RedirectToAction(nameof(Index));
    }
}
