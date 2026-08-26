using BarberFlow.Application.Validators;
using BarberFlow.Models;
using FluentAssertions;

namespace BarberFlow.Tests.Validators;

public class ServicoValidatorTests
{
    private readonly ServicoValidator _validator = new();

    [Fact]
    public void DeveAceitarServicoValido()
    {
        var servico = new Servico
        {
            Nome = "Corte tradicional",
            Descricao = "Corte com acabamento.",
            DuracaoMinutos = 45,
            Preco = 50
        };

        var resultado = _validator.Validate(servico);

        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(16)]
    [InlineData(481)]
    public void DeveRejeitarDuracaoInvalida(int duracao)
    {
        var servico = new Servico { Nome = "Corte", DuracaoMinutos = duracao, Preco = 50 };

        var resultado = _validator.Validate(servico);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(x => x.PropertyName == nameof(Servico.DuracaoMinutos));
    }
}
