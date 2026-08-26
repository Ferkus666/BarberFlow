using BarberFlow.Enums;
using BarberFlow.Models;
using BarberFlow.Services;
using BarberFlow.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace BarberFlow.Tests.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task ObterDashboardAsync_DeveCalcularFaturamentosEIndicadores()
    {
        await using var context = TestDbContextFactory.Create();
        var profissional = new Profissional { Nome = "Barbeiro", Ativo = true };
        var cliente = new Cliente { Nome = "Cliente", Email = "cliente@teste.com" };
        var servico = new Servico { Nome = "Corte", DuracaoMinutos = 30, Preco = 60, Ativo = true };
        context.AddRange(profissional, cliente, servico);

        context.Reservas.AddRange(
            CriarReserva(cliente, profissional, servico, DateTime.Today.AddHours(10), StatusReserva.Concluida),
            CriarReserva(cliente, profissional, servico, DateTime.Today.AddDays(1).AddHours(10), StatusReserva.Confirmada),
            CriarReserva(cliente, profissional, servico, DateTime.Today.AddDays(-1).AddHours(10), StatusReserva.Cancelada));
        await context.SaveChangesAsync();

        var sut = new DashboardService(context, NullLogger<DashboardService>.Instance);
        var resultado = await sut.ObterDashboardAsync();

        resultado.FaturamentoRealizado.Should().Be(60);
        resultado.FaturamentoPrevisto.Should().Be(60);
        resultado.Confirmadas.Should().Be(1);
        resultado.TotalClientes.Should().Be(1);
        resultado.ProfissionaisAtivos.Should().Be(1);
        resultado.TaxaCancelamento.Should().Be(50);
    }

    private static Reserva CriarReserva(
        Cliente cliente,
        Profissional profissional,
        Servico servico,
        DateTime inicio,
        StatusReserva status) => new()
    {
        Cliente = cliente,
        Profissional = profissional,
        Servico = servico,
        DataHoraInicio = inicio,
        DataHoraFim = inicio.AddMinutes(servico.DuracaoMinutos),
        Status = status
    };
}
