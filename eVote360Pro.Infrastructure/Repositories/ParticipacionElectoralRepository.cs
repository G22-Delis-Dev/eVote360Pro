using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class ParticipacionElectoralRepository : GenericRepository<ParticipacionElectoral>, IParticipacionElectoralRepository
{
    public ParticipacionElectoralRepository(AppDbContext context) : base(context) { }

    public async Task<bool> CiudadanoYaVotóAsync(int ciudadanoId, int eleccionId)
    {
        return await _dbSet.AnyAsync(p => p.CiudadanoId == ciudadanoId && p.EleccionId == eleccionId);
    }

    public async Task<int> ContarParticipantesPorEleccionAsync(int eleccionId)
    {
        return await _dbSet.CountAsync(p => p.EleccionId == eleccionId);
    }
}