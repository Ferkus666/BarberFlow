using BarberFlow.Data;
using BarberFlow.Interfaces;
using BarberFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Repositories;

public class ServicoRepository : IServicoRepository
{
    private readonly BarberFlowDbContext _context;

    public ServicoRepository(BarberFlowDbContext context) => _context = context;

    public async Task<IReadOnlyList<Servico>> ObterAtivosAsync() =>
        await _context.Servicos.AsNoTracking().Where(s => s.Ativo).OrderBy(s => s.Nome).ToListAsync();

    public async Task<Servico?> ObterPorIdAsync(int id) =>
        await _context.Servicos.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && s.Ativo);
}
