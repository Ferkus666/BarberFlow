using BarberFlow.Data;
using BarberFlow.ViewModels.Profissionais;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

public class ProfissionaisController : Controller
{
    private readonly BarberFlowDbContext _context;

    public ProfissionaisController(BarberFlowDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var profissionais = await _context.Profissionais.AsNoTracking()
            .Where(p => p.Ativo)
            .Include(p => p.ProfissionalServicos).ThenInclude(ps => ps.Servico)
            .Include(p => p.Avaliacoes)
            .OrderBy(p => p.Nome)
            .ToListAsync();
        return View(profissionais);
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var profissional = await _context.Profissionais.AsNoTracking()
            .Include(p => p.ProfissionalServicos).ThenInclude(ps => ps.Servico)
            .Include(p => p.HorariosTrabalho)
            .Include(p => p.Avaliacoes).ThenInclude(a => a.Cliente)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);
        if (profissional is null) return NotFound();

        var vm = new ProfissionalPublicoViewModel
        {
            Profissional = profissional,
            MediaAvaliacoes = profissional.Avaliacoes.Count == 0 ? 0 : profissional.Avaliacoes.Average(a => a.Nota),
            TotalAvaliacoes = profissional.Avaliacoes.Count,
            AvaliacoesRecentes = profissional.Avaliacoes.OrderByDescending(a => a.DataCriacao).Take(6).ToList()
        };
        return View(vm);
    }
}
