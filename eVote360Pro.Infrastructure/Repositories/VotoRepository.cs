using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class VotoRepository : GenericRepository<Voto>, IVotoRepository
{
    public VotoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Voto>> GetByEleccionYPuestoAsync(int eleccionId, int puestoId)
    {
        return await _dbSet
            .Where(v => v.EleccionId == eleccionId && v.PuestoElectivoId == puestoId)
            .ToListAsync();
    }

    public async Task<int> ContarVotosPorCandidatoAsync(int eleccionId, int puestoId, int? candidatoId)
    {
        return await _dbSet
            .CountAsync(v => v.EleccionId == eleccionId
                          && v.PuestoElectivoId == puestoId
                          && v.CandidatoId == candidatoId);
    }
}