using BarberFlow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Data;

public class BarberFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public BarberFlowDbContext(DbContextOptions<BarberFlowDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Profissional> Profissionais => Set<Profissional>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<ProfissionalServico> ProfissionalServicos => Set<ProfissionalServico>();
    public DbSet<HorarioTrabalho> HorariosTrabalho => Set<HorarioTrabalho>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProfissionalServico>()
            .HasKey(ps => new { ps.ProfissionalId, ps.ServicoId });

        modelBuilder.Entity<ProfissionalServico>()
            .HasOne(ps => ps.Profissional)
            .WithMany(p => p.ProfissionalServicos)
            .HasForeignKey(ps => ps.ProfissionalId);

        modelBuilder.Entity<ProfissionalServico>()
            .HasOne(ps => ps.Servico)
            .WithMany(s => s.ProfissionalServicos)
            .HasForeignKey(ps => ps.ServicoId);


        modelBuilder.Entity<Profissional>()
            .HasOne(p => p.Usuario)
            .WithOne()
            .HasForeignKey<Profissional>(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Profissional>()
            .HasIndex(p => p.UsuarioId)
            .IsUnique()
            .HasFilter("[UsuarioId] IS NOT NULL");

        modelBuilder.Entity<HorarioTrabalho>()
            .HasOne(h => h.Profissional)
            .WithMany(p => p.HorariosTrabalho)
            .HasForeignKey(h => h.ProfissionalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HorarioTrabalho>()
            .HasIndex(h => new { h.ProfissionalId, h.DiaSemana, h.HoraInicio, h.HoraFim });


        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.Cliente)
            .WithMany(c => c.Avaliacoes)
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.Profissional)
            .WithMany(p => p.Avaliacoes)
            .HasForeignKey(a => a.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.Reserva)
            .WithOne(r => r.Avaliacao)
            .HasForeignKey<Avaliacao>(a => a.ReservaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Avaliacao>()
            .HasIndex(a => a.ReservaId)
            .IsUnique();

        modelBuilder.Entity<Servico>().HasData(
            new Servico { Id = 1, Nome = "Corte masculino", Descricao = "Corte personalizado com acabamento.", DuracaoMinutos = 45, Preco = 50m, Ativo = true },
            new Servico { Id = 2, Nome = "Barba", Descricao = "Modelagem e acabamento da barba.", DuracaoMinutos = 30, Preco = 35m, Ativo = true },
            new Servico { Id = 3, Nome = "Corte e barba", Descricao = "Combo completo de corte e barba.", DuracaoMinutos = 75, Preco = 75m, Ativo = true }
        );
    }
}
