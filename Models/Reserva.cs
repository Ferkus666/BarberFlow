using BarberFlow.Enums;

namespace BarberFlow.Models;

public class Reserva
{
    public int Id { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public StatusReserva Status { get; set; } = StatusReserva.Pendente;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int ProfissionalId { get; set; }
    public Profissional Profissional { get; set; } = null!;

    public int ServicoId { get; set; }
    public Servico Servico { get; set; } = null!;

    public Avaliacao? Avaliacao { get; set; }
}
