using BarberFlow.Enums;

namespace BarberFlow.Application.DTOs;

public sealed class ReservaDto
{
    public int Id { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public StatusReserva Status { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Profissional { get; set; } = string.Empty;
    public string Servico { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
