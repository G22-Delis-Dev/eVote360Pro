using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class EleccionPuestoRepository : GenericRepository<EleccionPuesto>, IEleccionPuestoRepository
{
    public EleccionPuestoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EleccionPuesto>> GetByEleccionAsync(int eleccionId)
    {
        //  Usamos .Include() para traer los datos del Puesto Electivo.
        // Así, cuando la capa de Aplicación pida esto, no solo va a recibir id vacios,
        // sino que también tendrá acceso al nombre del puesto (ej. "Presidente", "Senador").
        return await _dbSet
            .Include(ep => ep.PuestoElectivo)
            .Where(ep => ep.EleccionId == eleccionId)
            .ToListAsync();
    }

    public async Task<bool> ExistePuestoEnEleccionAsync(int eleccionId, int puestoId)
    {
        // Valida que no agreguemos dos veces el mismo puesto (ej. "Presidente") a la misma elección.
        return await _dbSet
            .AnyAsync(ep => ep.EleccionId == eleccionId && ep.PuestoElectivoId == puestoId);
    }
}