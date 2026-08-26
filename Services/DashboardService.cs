using BarberFlow.Data;
using BarberFlow.Enums;
using BarberFlow.Interfaces;
using BarberFlow.ViewModels.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Services;

public class DashboardService : IDashboardService
{
    private readonly BarberFlowDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(BarberFlowDbContext context, ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardAdminViewModel> ObterDashboardAsync()
    {
        _logger.LogDebug("Iniciando cálculo do dashboard administrativo");
        var hoje = DateTime.Today;
        var inicioSemana = hoje.AddDays(-((7 + (int)hoje.DayOfWeek - (int)DayOfWeek.Monday) % 7));
        var fimSemana = inicioSemana.AddDays(7);
        var reservasBase = _context.Reservas.AsNoTracking();

        var totalEncerradas = await reservasBase.CountAsync(r =>
            r.Status == StatusReserva.Concluida ||
            r.Status == StatusReserva.Cancelada ||
            r.Status == StatusReserva.NaoCompareceu);
        var canceladas = await reservasBase.CountAsync(r => r.Status == StatusReserva.Cancelada);
        var naoCompareceu = await reservasBase.CountAsync(r => r.Status == StatusReserva.NaoCompareceu);

        var ultimosSeteDias = Enumerable.Range(0, 7).Select(i => hoje.AddDays(i - 6)).ToList();
        var contagensDias = await reservasBase
            .Where(r => r.DataHoraInicio >= hoje.AddDays(-6) && r.DataHoraInicio < hoje.AddDays(1))
            .GroupBy(r => r.DataHoraInicio.Date)
            .Select(g => new { Data = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.Data, x => x.Total);

        var porStatus = await reservasBase.GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Total = g.Count() }).ToListAsync();
        var porServico = await reservasBase.Where(r => r.Status != StatusReserva.Cancelada)
            .GroupBy(r => r.Servico.Nome).Select(g => new { Nome = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total).Take(5).ToListAsync();
        var porProfissional = await reservasBase.Where(r => r.Status != StatusReserva.Cancelada)
            .GroupBy(r => r.Profissional.Nome).Select(g => new { Nome = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total).Take(5).ToListAsync();

        return new DashboardAdminViewModel
        {
            AtendimentosHoje = await reservasBase.CountAsync(r => r.DataHoraInicio >= hoje && r.DataHoraInicio < hoje.AddDays(1) && r.Status != StatusReserva.Cancelada),
            ReservasSemana = await reservasBase.CountAsync(r => r.DataHoraInicio >= inicioSemana && r.DataHoraInicio < fimSemana && r.Status != StatusReserva.Cancelada),
            Pendentes = await reservasBase.CountAsync(r => r.Status == StatusReserva.Pendente),
            Confirmadas = await reservasBase.CountAsync(r => r.Status == StatusReserva.Confirmada),
            TotalClientes = await _context.Clientes.AsNoTracking().CountAsync(),
            ProfissionaisAtivos = await _context.Profissionais.AsNoTracking().CountAsync(p => p.Ativo),
            FaturamentoRealizado = await reservasBase.Where(r => r.Status == StatusReserva.Concluida).SumAsync(r => (decimal?)r.Servico.Preco) ?? 0,
            FaturamentoPrevisto = await reservasBase.Where(r => r.DataHoraInicio >= DateTime.Now && (r.Status == StatusReserva.Pendente || r.Status == StatusReserva.Confirmada)).SumAsync(r => (decimal?)r.Servico.Preco) ?? 0,
            TaxaCancelamento = totalEncerradas == 0 ? 0 : Math.Round(canceladas * 100m / totalEncerradas, 1),
            TaxaNaoComparecimento = totalEncerradas == 0 ? 0 : Math.Round(naoCompareceu * 100m / totalEncerradas, 1),
            ProximasReservas = await reservasBase.Include(r => r.Cliente).Include(r => r.Profissional).Include(r => r.Servico)
                .Where(r => r.DataHoraInicio >= DateTime.Now && r.Status != StatusReserva.Cancelada)
                .OrderBy(r => r.DataHoraInicio).Take(5).ToListAsync(),
            DiasLabels = ultimosSeteDias.Select(d => d.ToString("dd/MM")).ToList(),
            ReservasPorDia = ultimosSeteDias.Select(d => contagensDias.GetValueOrDefault(d, 0)).ToList(),
            StatusLabels = Enum.GetValues<StatusReserva>().Select(s => s == StatusReserva.NaoCompareceu ? "Não compareceu" : s.ToString()).ToList(),
            StatusValores = Enum.GetValues<StatusReserva>().Select(s => porStatus.FirstOrDefault(x => x.Status == s)?.Total ?? 0).ToList(),
            ServicosLabels = porServico.Select(x => x.Nome).ToList(),
            ServicosValores = porServico.Select(x => x.Total).ToList(),
            ProfissionaisLabels = porProfissional.Select(x => x.Nome).ToList(),
            ProfissionaisValores = porProfissional.Select(x => x.Total).ToList()
        };
    }
}
