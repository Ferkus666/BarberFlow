namespace BarberFlow.Application.DTOs;

public sealed class ServicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int DuracaoMinutos { get; set; }
    public decimal Preco { get; set; }
    public bool Ativo { get; set; }
}
