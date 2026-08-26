using System.ComponentModel.DataAnnotations;
namespace BarberFlow.Application.DTOs.Requests;
public sealed class ServicoRequest
{
    [Required, StringLength(100)] public string Nome { get; set; } = string.Empty;
    [StringLength(500)] public string? Descricao { get; set; }
    [Range(15,480)] public int DuracaoMinutos { get; set; }
    [Range(0.01,99999)] public decimal Preco { get; set; }
    public bool Ativo { get; set; } = true;
}
