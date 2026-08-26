using BarberFlow.Enums;
using BarberFlow.Models;
using BarberFlow.Services;
using BarberFlow.Tests.Helpers;
using FluentAssertions;

namespace BarberFlow.Tests.Services;

public class DisponibilidadeServiceTests
{
    [Fact]
    public async Task ObterHorariosAsync_DeveRetornarVazio_ParaDataPassada()
    {
        await using var context = TestDbContextFactory.Create();
        var sut = new DisponibilidadeService(context);

        var resultado = await sut.ObterHorariosAsync(1, 1, DateTime.Today.AddDays(-1));

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterHorariosAsync_DeveRespeitarDuracaoJornadaEConflitos()
    {
        await using var context = TestDbContextFactory.Create();
        var data = ProximaDataUtil();
        var profissional = new Profissional { Id = 10, Nome = "João", Ativo = true };
        var servico = new Servico { Id = 20, Nome = "Corte", DuracaoMinutos = 30, Preco = 50, Ativo = true };

        context.Profissionais.Add(profissional);
        context.Servicos.Add(servico);
        context.ProfissionalServicos.Add(new ProfissionalServico
        {
            ProfissionalId = profissional.Id,
            Profissional = profissional,
            ServicoId = servico.Id,
            Servico = servico
        });
        context.HorariosTrabalho.Add(new HorarioTrabalho
        {
            ProfissionalId = profissional.Id,
            Profissional = profissional,
            DiaSemana = data.DayOfWeek,
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFim = new TimeSpan(10, 15, 0),
            Ativo = true
        });
        context.Reservas.Add(new Reserva
        {
            Cliente = new Cliente { Id = 30, Nome = "Cliente", Email = "cliente@teste.com" },
            ClienteId = 30,
            Profissional = profissional,
            ProfissionalId = profissional.Id,
            Servico = servico,
            ServicoId = servico.Id,
            DataHoraInicio = data.AddHours(9).AddMinutes(15),
            DataHoraFim = data.AddHours(9).AddMinutes(45),
            Status = StatusReserva.Confirmada
        });
        await context.SaveChangesAsync();

        var sut = new DisponibilidadeService(context);
        var resultado = await sut.ObterHorariosAsync(profissional.Id, servico.Id, data);

        resultado.Select(x => x.Inicio.TimeOfDay)
            .Should().BeEquivalentTo(new[] { new TimeSpan(9, 45, 0) });
    }

    [Fact]
    public async Task ObterHorariosAsync_DeveIgnorarReservaCancelada()
    {
        await using var context = TestDbContextFactory.Create();
        var data = ProximaDataUtil();
        var profissional = new Profissional { Id = 11, Nome = "Carlos", Ativo = true };
        var servico = new Servico { Id = 21, Nome = "Barba", DuracaoMinutos = 30, Preco = 35, Ativo = true };

        context.AddRange(profissional, servico);
        context.ProfissionalServicos.Add(new ProfissionalServico { Profissional = profissional, Servico = servico });
        context.HorariosTrabalho.Add(new HorarioTrabalho
        {
            Profissional = profissional,
            DiaSemana = data.DayOfWeek,
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFim = new TimeSpan(9, 30, 0),
            Ativo = true
        });
        context.Reservas.Add(new Reserva
        {
            Cliente = new Cliente { Nome = "Cliente", Email = "cancelada@teste.com" },
            Profissional = profissional,
            Servico = servico,
            DataHoraInicio = data.AddHours(9),
            DataHoraFim = data.AddHours(9).AddMinutes(30),
            Status = StatusReserva.Cancelada
        });
        await context.SaveChangesAsync();

        var sut = new DisponibilidadeService(context);
        var resultado = await sut.ObterHorariosAsync(profissional.Id, servico.Id, data);

        resultado.Should().ContainSingle();
        resultado[0].Inicio.TimeOfDay.Should().Be(new TimeSpan(9, 0, 0));
    }

    private static DateTime ProximaDataUtil()
    {
        var data = DateTime.Today.AddDays(2);
        return data;
    }
}
