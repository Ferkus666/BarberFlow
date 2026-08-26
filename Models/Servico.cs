using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberFlow.Models;

public class Servico
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Range(1, 480)]
    public int DuracaoMinutos { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0.01, 99999)]
    public decimal Preco { get; set; }

    public bool Ativo { get; set; } = true;

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<ProfissionalServico> ProfissionalServicos { get; set; } = new List<ProfissionalServico>();
}
