using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.ViewModels.Relatorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

[Authorize(Roles = IdentitySeed.AdministradorRole)]
public class RelatoriosAdminController : Controller
{
    private readonly BarberFlowDbContext _context;
    public RelatoriosAdminController(BarberFlowDbContext context) => _context = context;

    public async Task<IActionResult> Index(RelatorioReservasViewModel filtro)
    {
        var query = _context.Reservas.AsNoTracking()
            .Include(r => r.Cliente).Include(r => r.Profissional).Include(r => r.Servico).AsQueryable();

        if (filtro.DataInicial.HasValue) query = query.Where(r => r.DataHoraInicio >= filtro.DataInicial.Value.Date);
        if (filtro.DataFinal.HasValue) query = query.Where(r => r.DataHoraInicio < filtro.DataFinal.Value.Date.AddDays(1));
        if (filtro.ProfissionalId.HasValue) query = query.Where(r => r.ProfissionalId == filtro.ProfissionalId);
        if (filtro.ServicoId.HasValue) query = query.Where(r => r.ServicoId == filtro.ServicoId);
        if (filtro.Status.HasValue) query = query.Where(r => r.Status == filtro.Status);

        filtro.Reservas = await query.OrderByDescending(r => r.DataHoraInicio).ToListAsync();
        filtro.Profissionais = await _context.Profissionais.AsNoTracking().OrderBy(p => p.Nome)
            .Select(p => new SelectListItem(p.Nome, p.Id.ToString())).ToListAsync();
        filtro.Servicos = await _context.Servicos.AsNoTracking().OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString())).ToListAsync();
        return View(filtro);
    }
}
