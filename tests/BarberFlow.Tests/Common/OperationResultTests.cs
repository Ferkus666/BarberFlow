using BarberFlow.Application.Common;
using FluentAssertions;

namespace BarberFlow.Tests.Common;

public class OperationResultTests
{
    [Fact]
    public void Ok_DeveCriarResultadoBemSucedido()
    {
        var resultado = OperationResult<int>.Ok(42, "Tudo certo");

        resultado.Success.Should().BeTrue();
        resultado.Data.Should().Be(42);
        resultado.Message.Should().Be("Tudo certo");
        resultado.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Fail_DeveConterErro()
    {
        var resultado = OperationResult<int>.Fail("Falhou");

        resultado.Success.Should().BeFalse();
        resultado.Errors.Should().ContainSingle().Which.Should().Be("Falhou");
    }
}
