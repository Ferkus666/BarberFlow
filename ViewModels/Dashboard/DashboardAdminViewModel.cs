using BarberFlow.Models;

namespace BarberFlow.ViewModels.Dashboard;

public class DashboardAdminViewModel
{
    public int AtendimentosHoje { get; set; }
    public int ReservasSemana { get; set; }
    public int Pendentes { get; set; }
    public int Confirmadas { get; set; }
    public int TotalClientes { get; set; }
    public int ProfissionaisAtivos { get; set; }
    public decimal FaturamentoRealizado { get; set; }
    public decimal FaturamentoPrevisto { get; set; }
    public decimal TaxaCancelamento { get; set; }
    public decimal TaxaNaoComparecimento { get; set; }
    public List<Reserva> ProximasReservas { get; set; } = [];
    public List<string> DiasLabels { get; set; } = [];
    public List<int> ReservasPorDia { get; set; } = [];
    public List<string> StatusLabels { get; set; } = [];
    public List<int> StatusValores { get; set; } = [];
    public List<string> ServicosLabels { get; set; } = [];
    public List<int> ServicosValores { get; set; } = [];
    public List<string> ProfissionaisLabels { get; set; } = [];
    public List<int> ProfissionaisValores { get; set; } = [];
}
