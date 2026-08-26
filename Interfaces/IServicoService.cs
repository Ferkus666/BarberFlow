using BarberFlow.Models;

namespace BarberFlow.Interfaces;

public interface IServicoService
{
    Task<IReadOnlyList<Servico>> ListarAtivosAsync();
    Task<Servico?> BuscarPorIdAsync(int id);
}
