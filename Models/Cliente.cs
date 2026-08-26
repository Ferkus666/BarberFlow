using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? Telefone { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
}
