using BarberFlow.Interfaces;
using BarberFlow.Models;

namespace BarberFlow.Services;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _repository;

    public ServicoService(IServicoRepository repository) => _repository = repository;

    public Task<IReadOnlyList<Servico>> ListarAtivosAsync() => _repository.ObterAtivosAsync();
    public Task<Servico?> BuscarPorIdAsync(int id) => _repository.ObterPorIdAsync(id);
}
