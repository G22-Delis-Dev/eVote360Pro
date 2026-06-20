using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class CandidatoRepository : GenericRepository<Candidato>, ICandidatoRepository
{
    public CandidatoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Candidato>> GetActivosByPartidoAsync(int partidoId)
    {
        // Trae todos los candidatos que pertenecen a un partido específico y que están activos
        return await _dbSet
            .Where(c => c.PartidoPoliticoId == partidoId && c.Activo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidato>> GetActivosByPartidosAsync(IEnumerable<int> partidoIds)
    {
        // Trae candidatos activos de múltiples partidos (para candidatos aliados)
        // Incluye el partido de origen para poder mostrarlo en el dropdown
        return await _dbSet
            .Include(c => c.PartidoPolitico)
            .Where(c => partidoIds.Contains(c.PartidoPoliticoId) && c.Activo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidato>> GetByPartidoConPuestosAsync(int partidoId)
    {
        // Trae candidatos del partido incluyendo sus asignaciones de puesto (activas)
        // y el partido, necesario para el mapeo completo de la vista
        return await _dbSet
            .Include(c => c.PartidoPolitico)
            .Include(c => c.AsignacionesPuestos.Where(a => a.Activo))
                .ThenInclude(a => a.PuestoElectivo)
            .Where(c => c.PartidoPoliticoId == partidoId)
            .ToListAsync();
    }

    public async Task<bool> EstaAsignadoAPuestoAsync(int candidatoId)
    {
        // Valida si el candidato tiene una asignación de puesto activa (no eliminada lógicamente)
        return await _dbSet
            .AnyAsync(c => c.Id == candidatoId && c.AsignacionesPuestos.Any(a => a.Activo));
    }

    public async Task<bool> ParticipoEnEleccionAsync(int candidatoId)
    {
        // Un candidato se considera que participó en una elección si tiene una asignación de puesto registrada.
        return await _dbSet
            .AnyAsync(c => c.Id == candidatoId && c.AsignacionesPuestos.Any());
    }
}
