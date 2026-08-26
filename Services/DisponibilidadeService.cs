using BarberFlow.Data;
using BarberFlow.Enums;
using BarberFlow.Interfaces;
using BarberFlow.ViewModels.Reservas;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Services;

public class DisponibilidadeService : IDisponibilidadeService
{
    private readonly BarberFlowDbContext _context;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(15);

    public DisponibilidadeService(BarberFlowDbContext context) => _context = context;

    public async Task<IReadOnlyList<HorarioDisponivelViewModel>> ObterHorariosAsync(int profissionalId, int servicoId, DateTime data)
    {
        data = data.Date;
        if (data < DateTime.Today) return [];

        var servico = await _context.Servicos.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == servicoId && s.Ativo);
        if (servico is null) return [];

        var atendeServico = await _context.ProfissionalServicos.AsNoTracking()
            .AnyAsync(ps => ps.ProfissionalId == profissionalId && ps.ServicoId == servicoId && ps.Profissional.Ativo);
        if (!atendeServico) return [];

        var jornadas = await _context.HorariosTrabalho.AsNoTracking()
            .Where(h => h.ProfissionalId == profissionalId && h.DiaSemana == data.DayOfWeek && h.Ativo)
            .OrderBy(h => h.HoraInicio)
            .ToListAsync();

        var inicioDia = data;
        var fimDia = data.AddDays(1);
        var reservas = await _context.Reservas.AsNoTracking()
            .Where(r => r.ProfissionalId == profissionalId &&
                        r.Status != StatusReserva.Cancelada &&
                        r.DataHoraInicio < fimDia && r.DataHoraFim > inicioDia)
            .ToListAsync();

        var duracao = TimeSpan.FromMinutes(servico.DuracaoMinutos);
        var agora = DateTime.Now;
        var resultado = new List<HorarioDisponivelViewModel>();

        foreach (var jornada in jornadas)
        {
            var cursor = data.Add(jornada.HoraInicio);
            var limite = data.Add(jornada.HoraFim);

            while (cursor + duracao <= limite)
            {
                var fim = cursor + duracao;
                var conflito = reservas.Any(r => cursor < r.DataHoraFim && fim > r.DataHoraInicio);
                if (!conflito && cursor > agora)
                    resultado.Add(new HorarioDisponivelViewModel { Inicio = cursor, Fim = fim });

                cursor = cursor.Add(Intervalo);
            }
        }

        return resultado;
    }

    public async Task<bool> HorarioEstaDisponivelAsync(int profissionalId, int servicoId, DateTime inicio)
    {
        var horarios = await ObterHorariosAsync(profissionalId, servicoId, inicio.Date);
        return horarios.Any(h => h.Inicio == inicio);
    }
}
