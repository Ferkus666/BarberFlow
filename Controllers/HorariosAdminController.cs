using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Models;
using BarberFlow.ViewModels.Profissionais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

[Authorize(Roles = IdentitySeed.AdministradorRole)]
public class HorariosAdminController : Controller
{
    private readonly BarberFlowDbContext _context;
    public HorariosAdminController(BarberFlowDbContext context) => _context = context;

    public async Task<IActionResult> Index(int profissionalId)
    {
        var profissional = await _context.Profissionais.AsNoTracking().FirstOrDefaultAsync(p => p.Id == profissionalId);
        if (profissional is null) return NotFound();
        ViewBag.Profissional = profissional;
        return View(await _context.HorariosTrabalho.AsNoTracking().Where(h => h.ProfissionalId == profissionalId)
            .OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio).ToListAsync());
    }

    public async Task<IActionResult> Criar(int profissionalId)
    {
        var profissional = await _context.Profissionais.AsNoTracking().FirstOrDefaultAsync(p => p.Id == profissionalId);
        if (profissional is null) return NotFound();
        return View(new HorarioTrabalhoViewModel { ProfissionalId = profissional.Id, ProfissionalNome = profissional.Nome });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(HorarioTrabalhoViewModel vm)
    {
        var profissional = await _context.Profissionais.AsNoTracking().FirstOrDefaultAsync(p => p.Id == vm.ProfissionalId);
        if (profissional is null) return NotFound();
        vm.ProfissionalNome = profissional.Nome;

        var conflito = await _context.HorariosTrabalho.AnyAsync(h => h.ProfissionalId == vm.ProfissionalId && h.DiaSemana == vm.DiaSemana && vm.HoraInicio < h.HoraFim && vm.HoraFim > h.HoraInicio);
        if (conflito) ModelState.AddModelError(string.Empty, "Já existe um período de trabalho que se sobrepõe a esse horário.");
        if (!ModelState.IsValid) return View(vm);

        _context.HorariosTrabalho.Add(new HorarioTrabalho { ProfissionalId = vm.ProfissionalId, DiaSemana = vm.DiaSemana, HoraInicio = vm.HoraInicio, HoraFim = vm.HoraFim });
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Horário adicionado com sucesso.";
        return RedirectToAction(nameof(Index), new { profissionalId = vm.ProfissionalId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id)
    {
        var horario = await _context.HorariosTrabalho.FindAsync(id);
        if (horario is null) return NotFound();
        var profissionalId = horario.ProfissionalId;
        _context.HorariosTrabalho.Remove(horario);
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Horário removido.";
        return RedirectToAction(nameof(Index), new { profissionalId });
    }
}
