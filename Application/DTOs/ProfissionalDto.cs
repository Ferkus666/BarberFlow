namespace BarberFlow.Application.DTOs;

public sealed class ProfissionalDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Biografia { get; set; }
    public string? Especialidade { get; set; }
    public int? AnosExperiencia { get; set; }
    public string? Instagram { get; set; }
    public string? FotoUrl { get; set; }
    public bool Ativo { get; set; }
    public IReadOnlyCollection<string> Servicos { get; set; } = Array.Empty<string>();
}
