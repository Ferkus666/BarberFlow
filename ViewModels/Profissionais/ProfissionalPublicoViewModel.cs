using BarberFlow.Models;

namespace BarberFlow.ViewModels.Profissionais;

public sealed class ProfissionalPublicoViewModel
{
    public Profissional Profissional { get; set; } = null!;
    public double MediaAvaliacoes { get; set; }
    public int TotalAvaliacoes { get; set; }
    public IReadOnlyList<Avaliacao> AvaliacoesRecentes { get; set; } = Array.Empty<Avaliacao>();
}
