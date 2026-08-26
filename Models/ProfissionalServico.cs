namespace BarberFlow.Models;

public class ProfissionalServico
{
    public int ProfissionalId { get; set; }
    public Profissional Profissional { get; set; } = null!;

    public int ServicoId { get; set; }
    public Servico Servico { get; set; } = null!;
}
