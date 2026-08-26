namespace BarberFlow.Application.DTOs;

public sealed class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public DateTime DataCadastro { get; set; }
}
