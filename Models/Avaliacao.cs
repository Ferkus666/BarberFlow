using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Models;

public class Avaliacao
{
    public int Id { get; set; }

    [Range(1, 5)]
    public int Nota { get; set; }

    [StringLength(500)]
    public string? Comentario { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int ProfissionalId { get; set; }
    public Profissional Profissional { get; set; } = null!;

    public int ReservaId { get; set; }
    public Reserva Reserva { get; set; } = null!;
}
