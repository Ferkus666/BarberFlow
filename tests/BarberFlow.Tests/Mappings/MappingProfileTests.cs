using AutoMapper;
using BarberFlow.Application.DTOs;
using BarberFlow.Application.Mappings;
using BarberFlow.Enums;
using BarberFlow.Models;
using FluentAssertions;

namespace BarberFlow.Tests.Mappings;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        configuration.AssertConfigurationIsValid();
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void DeveMapearReservaParaDto()
    {
        var reserva = new Reserva
        {
            Id = 7,
            Cliente = new Cliente { Nome = "Fernando" },
            Profissional = new Profissional { Nome = "João" },
            Servico = new Servico { Nome = "Corte", Preco = 50 },
            DataHoraInicio = new DateTime(2030, 1, 2, 10, 0, 0),
            DataHoraFim = new DateTime(2030, 1, 2, 10, 45, 0),
            Status = StatusReserva.Confirmada
        };

        var dto = _mapper.Map<ReservaDto>(reserva);

        dto.Id.Should().Be(7);
        dto.Cliente.Should().Be("Fernando");
        dto.Profissional.Should().Be("João");
        dto.Servico.Should().Be("Corte");
        dto.Valor.Should().Be(50);
    }
}
