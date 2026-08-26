using BarberFlow.Models;

namespace BarberFlow.Interfaces;

public interface IServicoRepository
{
    Task<IReadOnlyList<Servico>> ObterAtivosAsync();
    Task<Servico?> ObterPorIdAsync(int id);
}
