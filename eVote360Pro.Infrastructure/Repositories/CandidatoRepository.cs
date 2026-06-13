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

    public async Task<bool> EstaAsignadoAPuestoAsync(int candidatoId)
    {
        // Valida si el candidato ya fue inscrito en alguna boleta (asignado a un puesto)
        return await _dbSet
            .AnyAsync(c => c.Id == candidatoId && c.AsignacionesPuestos.Any());
    }

    public async Task<bool> ParticipoEnEleccionAsync(int candidatoId)
    {
        // Un candidato se considera que participó en una elección si tiene una asignación de puesto registrada.
        return await _dbSet
            .AnyAsync(c => c.Id == candidatoId && c.AsignacionesPuestos.Any());
    }
}