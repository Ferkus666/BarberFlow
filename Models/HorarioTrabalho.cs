using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Models;

public class HorarioTrabalho
{
    public int Id { get; set; }

    [Required]
    public DayOfWeek DiaSemana { get; set; }

    [Required]
    public TimeSpan HoraInicio { get; set; }

    [Required]
    public TimeSpan HoraFim { get; set; }

    public bool Ativo { get; set; } = true;

    public int ProfissionalId { get; set; }
    public Profissional Profissional { get; set; } = null!;
}
