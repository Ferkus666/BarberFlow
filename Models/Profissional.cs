using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Models;

public class Profissional
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Biografia { get; set; }

    [StringLength(120)]
    public string? Especialidade { get; set; }

    [Range(0, 60)]
    public int? AnosExperiencia { get; set; }

    [StringLength(120)]
    public string? Instagram { get; set; }

    [StringLength(300)]
    public string? FotoUrl { get; set; }

    public bool Ativo { get; set; } = true;

    public string? UsuarioId { get; set; }
    public ApplicationUser? Usuario { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<ProfissionalServico> ProfissionalServicos { get; set; } = new List<ProfissionalServico>();
    public ICollection<HorarioTrabalho> HorariosTrabalho { get; set; } = new List<HorarioTrabalho>();
    public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
}
