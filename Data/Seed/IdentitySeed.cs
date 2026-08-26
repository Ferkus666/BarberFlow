using BarberFlow.Models;
using Microsoft.AspNetCore.Identity;

namespace BarberFlow.Data.Seed;

public static class IdentitySeed
{
    public const string ClienteRole = "Cliente";
    public const string ProfissionalRole = "Profissional";
    public const string AdministradorRole = "Administrador";

    public static async Task InicializarAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { ClienteRole, ProfissionalRole, AdministradorRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var email = configuration["SeedAdmin:Email"];
        var senha = configuration["SeedAdmin:Password"];
        var nome = configuration["SeedAdmin:Name"] ?? "Administrador BarberFlow";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return;

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                Nome = nome,
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(admin, senha);
            if (!resultado.Succeeded)
                throw new InvalidOperationException("Não foi possível criar o administrador inicial: " +
                    string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(admin, AdministradorRole))
            await userManager.AddToRoleAsync(admin, AdministradorRole);
    }
}
