using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

[Authorize(Roles = IdentitySeed.AdministradorRole)]
public class ServicosAdminController : Controller
{
    private readonly BarberFlowDbContext _context;
    public ServicosAdminController(BarberFlowDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.Servicos.AsNoTracking().OrderBy(s => s.Nome).ToListAsync());
    public IActionResult Criar() => View("Formulario", new Servico());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Servico servico)
    {
        if (!ModelState.IsValid) return View("Formulario", servico);
        servico.Nome = servico.Nome.Trim(); servico.Descricao = servico.Descricao?.Trim();
        _context.Add(servico); await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Serviço cadastrado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);
        return servico is null ? NotFound() : View("Formulario", servico);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Servico servico)
    {
        if (id != servico.Id) return BadRequest();
        if (!ModelState.IsValid) return View("Formulario", servico);
        var atual = await _context.Servicos.FindAsync(id);
        if (atual is null) return NotFound();
        atual.Nome = servico.Nome.Trim(); atual.Descricao = servico.Descricao?.Trim(); atual.DuracaoMinutos = servico.DuracaoMinutos; atual.Preco = servico.Preco; atual.Ativo = servico.Ativo;
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Serviço atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AlternarStatus(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);
        if (servico is null) return NotFound();
        servico.Ativo = !servico.Ativo; await _context.SaveChangesAsync();
        TempData["Sucesso"] = servico.Ativo ? "Serviço ativado." : "Serviço desativado.";
        return RedirectToAction(nameof(Index));
    }
}
