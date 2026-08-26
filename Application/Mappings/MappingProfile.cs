using AutoMapper;
using BarberFlow.Application.DTOs;
using BarberFlow.Models;

namespace BarberFlow.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Servico, ServicoDto>();
        CreateMap<Cliente, ClienteDto>();
        CreateMap<Profissional, ProfissionalDto>()
            .ForMember(d => d.Servicos, o => o.MapFrom(s => s.ProfissionalServicos.Select(ps => ps.Servico.Nome)));
        CreateMap<Reserva, ReservaDto>()
            .ForMember(d => d.Cliente, o => o.MapFrom(s => s.Cliente.Nome))
            .ForMember(d => d.Profissional, o => o.MapFrom(s => s.Profissional.Nome))
            .ForMember(d => d.Servico, o => o.MapFrom(s => s.Servico.Nome))
            .ForMember(d => d.Valor, o => o.MapFrom(s => s.Servico.Preco));
    }
}
