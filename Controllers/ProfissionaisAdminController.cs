using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Models;
using BarberFlow.ViewModels.Profissionais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers;

[Authorize(Roles = IdentitySeed.AdministradorRole)]
public class ProfissionaisAdminController : Controller
{
    private readonly BarberFlowDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public ProfissionaisAdminController(BarberFlowDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var profissionais = await _context.Profissionais.AsNoTracking()
            .Include(p => p.Usuario)
            .Include(p => p.ProfissionalServicos).ThenInclude(ps => ps.Servico)
            .Include(p => p.HorariosTrabalho)
            .OrderBy(p => p.Nome).ToListAsync();
        return View(profissionais);
    }

    public async Task<IActionResult> Criar()
    {
        var vm = new ProfissionalFormViewModel();
        await CarregarServicosAsync(vm);
        return View("Formulario", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(ProfissionalFormViewModel vm)
    {
        ValidarFoto(vm);
        if (string.IsNullOrWhiteSpace(vm.Email)) ModelState.AddModelError(nameof(vm.Email), "Informe o e-mail de acesso.");
        if (string.IsNullOrWhiteSpace(vm.Senha)) ModelState.AddModelError(nameof(vm.Senha), "Informe a senha inicial.");
        if (!ModelState.IsValid) { await CarregarServicosAsync(vm); return View("Formulario", vm); }

        var email = vm.Email!.Trim();
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            ModelState.AddModelError(nameof(vm.Email), "Já existe um usuário com este e-mail.");
            await CarregarServicosAsync(vm); return View("Formulario", vm);
        }

        var usuario = new ApplicationUser { Nome = vm.Nome.Trim(), UserName = email, Email = email, EmailConfirmed = true };
        var resultado = await _userManager.CreateAsync(usuario, vm.Senha!);
        if (!resultado.Succeeded)
        {
            foreach (var erro in resultado.Errors) ModelState.AddModelError(string.Empty, erro.Description);
            await CarregarServicosAsync(vm); return View("Formulario", vm);
        }
        await _userManager.AddToRoleAsync(usuario, IdentitySeed.ProfissionalRole);

        var profissional = new Profissional
        {
            Nome = vm.Nome.Trim(), Biografia = vm.Biografia?.Trim(), Especialidade = vm.Especialidade?.Trim(),
            AnosExperiencia = vm.AnosExperiencia, Instagram = NormalizarInstagram(vm.Instagram),
            FotoUrl = await SalvarFotoAsync(vm.Foto), Ativo = vm.Ativo, UsuarioId = usuario.Id,
            ProfissionalServicos = vm.ServicosSelecionados.Distinct().Select(id => new ProfissionalServico { ServicoId = id }).ToList()
        };
        _context.Profissionais.Add(profissional);
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Profissional e acesso cadastrados com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var profissional = await _context.Profissionais.Include(p => p.Usuario).Include(p => p.ProfissionalServicos).FirstOrDefaultAsync(p => p.Id == id);
        if (profissional is null) return NotFound();
        var vm = new ProfissionalFormViewModel
        {
            Id = profissional.Id, Nome = profissional.Nome, Biografia = profissional.Biografia, Especialidade = profissional.Especialidade,
            AnosExperiencia = profissional.AnosExperiencia, Instagram = profissional.Instagram, FotoUrlAtual = profissional.FotoUrl, Ativo = profissional.Ativo,
            Email = profissional.Usuario?.Email,
            ServicosSelecionados = profissional.ProfissionalServicos.Select(ps => ps.ServicoId).ToList()
        };
        await CarregarServicosAsync(vm);
        return View("Formulario", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, ProfissionalFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        var profissional = await _context.Profissionais.Include(p => p.Usuario).Include(p => p.ProfissionalServicos).FirstOrDefaultAsync(p => p.Id == id);
        if (profissional is null) return NotFound();
        ValidarFoto(vm);
        if (!ModelState.IsValid) { vm.FotoUrlAtual = profissional.FotoUrl; await CarregarServicosAsync(vm); return View("Formulario", vm); }

        profissional.Nome = vm.Nome.Trim();
        profissional.Biografia = vm.Biografia?.Trim();
        profissional.Especialidade = vm.Especialidade?.Trim();
        profissional.AnosExperiencia = vm.AnosExperiencia;
        profissional.Instagram = NormalizarInstagram(vm.Instagram);
        if (vm.Foto is not null)
        {
            var fotoAnterior = profissional.FotoUrl;
            profissional.FotoUrl = await SalvarFotoAsync(vm.Foto);
            RemoverFoto(fotoAnterior);
        }
        profissional.Ativo = vm.Ativo;
        profissional.ProfissionalServicos.Clear();
        foreach (var servicoId in vm.ServicosSelecionados.Distinct())
            profissional.ProfissionalServicos.Add(new ProfissionalServico { ProfissionalId = id, ServicoId = servicoId });

        if (profissional.Usuario is not null)
        {
            profissional.Usuario.Nome = profissional.Nome;
            if (!string.IsNullOrWhiteSpace(vm.Email) && !string.Equals(profissional.Usuario.Email, vm.Email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var outro = await _userManager.FindByEmailAsync(vm.Email.Trim());
                if (outro is not null && outro.Id != profissional.Usuario.Id)
                {
                    ModelState.AddModelError(nameof(vm.Email), "Já existe um usuário com este e-mail.");
                    await CarregarServicosAsync(vm); return View("Formulario", vm);
                }
                profissional.Usuario.Email = vm.Email.Trim();
                profissional.Usuario.UserName = vm.Email.Trim();
            }
            await _userManager.UpdateAsync(profissional.Usuario);
            if (!string.IsNullOrWhiteSpace(vm.Senha))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(profissional.Usuario);
                var troca = await _userManager.ResetPasswordAsync(profissional.Usuario, token, vm.Senha);
                if (!troca.Succeeded)
                {
                    foreach (var erro in troca.Errors) ModelState.AddModelError(string.Empty, erro.Description);
                    await CarregarServicosAsync(vm); return View("Formulario", vm);
                }
            }
        }

        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Profissional atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AlternarStatus(int id)
    {
        var profissional = await _context.Profissionais.FindAsync(id);
        if (profissional is null) return NotFound();
        profissional.Ativo = !profissional.Ativo;
        await _context.SaveChangesAsync();
        TempData["Sucesso"] = profissional.Ativo ? "Profissional ativado." : "Profissional desativado.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidarFoto(ProfissionalFormViewModel vm)
    {
        if (vm.Foto is null || vm.Foto.Length == 0) return;
        var extensao = Path.GetExtension(vm.Foto.FileName);
        var permitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
        if (!permitidas.Contains(extensao))
            ModelState.AddModelError(nameof(vm.Foto), "Formato não permitido. Use JPG, PNG ou WEBP.");
        if (vm.Foto.Length > 3 * 1024 * 1024)
            ModelState.AddModelError(nameof(vm.Foto), "A foto deve ter no máximo 3 MB.");
    }

    private async Task<string?> SalvarFotoAsync(IFormFile? foto)
    {
        if (foto is null || foto.Length == 0) return null;
        var extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();
        var permitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
        if (!permitidas.Contains(extensao) || foto.Length > 3 * 1024 * 1024) return null;

        var pasta = Path.Combine(_environment.WebRootPath, "uploads", "profissionais");
        Directory.CreateDirectory(pasta);
        var nome = $"{Guid.NewGuid():N}{extensao}";
        var caminho = Path.Combine(pasta, nome);
        await using var stream = System.IO.File.Create(caminho);
        await foto.CopyToAsync(stream);
        return $"/uploads/profissionais/{nome}";
    }

    private void RemoverFoto(string? fotoUrl)
    {
        if (string.IsNullOrWhiteSpace(fotoUrl) || !fotoUrl.StartsWith("/uploads/profissionais/", StringComparison.OrdinalIgnoreCase)) return;
        var caminho = Path.Combine(_environment.WebRootPath, fotoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(caminho)) System.IO.File.Delete(caminho);
    }

    private static string? NormalizarInstagram(string? instagram)
    {
        if (string.IsNullOrWhiteSpace(instagram)) return null;
        return instagram.Trim().TrimStart('@');
    }

    private async Task CarregarServicosAsync(ProfissionalFormViewModel vm) =>
        vm.ServicosDisponiveis = await _context.Servicos.AsNoTracking().Where(s => s.Ativo).OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString(), vm.ServicosSelecionados.Contains(s.Id))).ToListAsync();
}
