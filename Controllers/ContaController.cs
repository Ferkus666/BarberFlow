using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Models;
using BarberFlow.ViewModels.Conta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Controllers;

public class ContaController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly BarberFlowDbContext _context;
    private readonly ILogger<ContaController> _logger;

    public ContaController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        BarberFlowDbContext context,
        ILogger<ContaController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = await _userManager.FindByEmailAsync(model.Email.Trim());
        if (usuario is null)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        var resultado = await _signInManager.PasswordSignInAsync(usuario, model.Senha, model.LembrarMe, lockoutOnFailure: true);
        if (!resultado.Succeeded)
        {
            _logger.LogWarning("Falha de login para o e-mail {Email}. Bloqueado: {Bloqueado}", model.Email, resultado.IsLockedOut);
            ModelState.AddModelError(string.Empty, resultado.IsLockedOut
                ? "Conta temporariamente bloqueada por muitas tentativas. Tente novamente mais tarde."
                : "E-mail ou senha inválidos.");
            return View(model);
        }

        _logger.LogInformation("Login realizado para o usuário {UsuarioId}", usuario.Id);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Registro() => View(new RegistroViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim();
        var usuario = new ApplicationUser
        {
            Nome = model.Nome.Trim(),
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var resultado = await _userManager.CreateAsync(usuario, model.Senha);
        if (!resultado.Succeeded)
        {
            _logger.LogWarning("Falha ao criar usuário para o e-mail {Email}. Erros: {QuantidadeErros}", model.Email, resultado.Errors.Count());
            foreach (var erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(usuario, IdentitySeed.ClienteRole);

        _context.Clientes.Add(new Cliente
        {
            Nome = usuario.Nome,
            Email = email
        });
        await _context.SaveChangesAsync();

        await _signInManager.SignInAsync(usuario, isPersistent: false);
        _logger.LogInformation("Cliente criado com usuário {UsuarioId}", usuario.Id);
        TempData["Sucesso"] = "Conta criada com sucesso. Bem-vindo ao BarberFlow!";
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        _logger.LogInformation("Logout realizado pelo usuário {Usuario}", User.Identity?.Name);
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AcessoNegado() => View();
}
