using BarberFlow.ViewModels.Reservas;

namespace BarberFlow.Interfaces;

public interface IDisponibilidadeService
{
    Task<IReadOnlyList<HorarioDisponivelViewModel>> ObterHorariosAsync(int profissionalId, int servicoId, DateTime data);
    Task<bool> HorarioEstaDisponivelAsync(int profissionalId, int servicoId, DateTime inicio);
}
