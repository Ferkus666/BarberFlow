using BarberFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Data.Seed;

public static class DemoDataSeed
{
    private const string SenhaDemo = "Demo123";

    private sealed record ProfissionalDemo(
        string Nome,
        string Email,
        string Especialidade,
        string Biografia,
        int AnosExperiencia,
        string Instagram,
        int[] Servicos);

    private static readonly ProfissionalDemo[] Profissionais =
    [
        new("Rafael Martins", "rafael@barberflow.local", "Fade e cortes modernos",
            "Especialista em degradês, cortes modernos e acabamento com navalha.", 8, "@rafael.barberflow", [1, 3]),
        new("Lucas Almeida", "lucas@barberflow.local", "Barba e acabamento",
            "Foco em modelagem de barba, acabamento e atendimento clássico de barbearia.", 6, "@lucas.barberflow", [1, 2, 3]),
        new("Matheus Rocha", "matheus@barberflow.local", "Corte clássico",
            "Especialista em cortes clássicos, tesoura e visuais executivos.", 10, "@matheus.barberflow", [1, 3]),
        new("Diego Santos", "diego@barberflow.local", "Corte e barba",
            "Atendimento completo para quem procura praticidade, corte e barba no mesmo horário.", 7, "@diego.barberflow", [1, 2, 3])
    ];

    public static async Task InicializarAsync(IServiceProvider services, IConfiguration configuration)
    {
        if (!configuration.GetValue("SeedDemoData:Enabled", true))
            return;

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BarberFlowDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var servicosDisponiveis = await db.Servicos.AsNoTracking().Select(s => s.Id).ToListAsync();
        if (servicosDisponiveis.Count == 0)
            return;

        foreach (var item in Profissionais)
        {
            var usuario = await userManager.FindByEmailAsync(item.Email);
            if (usuario is null)
            {
                usuario = new ApplicationUser
                {
                    Nome = item.Nome,
                    UserName = item.Email,
                    Email = item.Email,
                    EmailConfirmed = true
                };

                var criacao = await userManager.CreateAsync(usuario, SenhaDemo);
                if (!criacao.Succeeded)
                    throw new InvalidOperationException($"Falha ao criar usuário demo {item.Email}: " +
                        string.Join("; ", criacao.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(usuario, IdentitySeed.ProfissionalRole))
                await userManager.AddToRoleAsync(usuario, IdentitySeed.ProfissionalRole);

            var profissional = await db.Profissionais
                .Include(p => p.ProfissionalServicos)
                .Include(p => p.HorariosTrabalho)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuario.Id || p.Nome == item.Nome);

            if (profissional is null)
            {
                profissional = new Profissional
                {
                    Nome = item.Nome,
                    UsuarioId = usuario.Id,
                    Especialidade = item.Especialidade,
                    Biografia = item.Biografia,
                    AnosExperiencia = item.AnosExperiencia,
                    Instagram = item.Instagram,
                    Ativo = true
                };
                db.Profissionais.Add(profissional);
                await db.SaveChangesAsync();
            }
            else
            {
                profissional.UsuarioId ??= usuario.Id;
                profissional.Especialidade ??= item.Especialidade;
                profissional.Biografia ??= item.Biografia;
                profissional.AnosExperiencia ??= item.AnosExperiencia;
                profissional.Instagram ??= item.Instagram;
                profissional.Ativo = true;
            }

            foreach (var servicoId in item.Servicos.Where(servicosDisponiveis.Contains))
            {
                if (profissional.ProfissionalServicos.All(ps => ps.ServicoId != servicoId))
                {
                    profissional.ProfissionalServicos.Add(new ProfissionalServico
                    {
                        ProfissionalId = profissional.Id,
                        ServicoId = servicoId
                    });
                }
            }

            // Jornada padrão de demonstração: segunda a sábado, das 09:00 às 18:00.
            foreach (var dia in new[]
                     {
                         DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                         DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
                     })
            {
                if (profissional.HorariosTrabalho.All(h => h.DiaSemana != dia))
                {
                    profissional.HorariosTrabalho.Add(new HorarioTrabalho
                    {
                        ProfissionalId = profissional.Id,
                        DiaSemana = dia,
                        HoraInicio = new TimeSpan(9, 0, 0),
                        HoraFim = new TimeSpan(18, 0, 0),
                        Ativo = true
                    });
                }
            }
        }

        await db.SaveChangesAsync();
    }
}
