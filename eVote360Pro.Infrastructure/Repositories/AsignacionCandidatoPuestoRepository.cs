using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class AsignacionCandidatoPuestoRepository : GenericRepository<AsignacionCandidatoPuesto>, IAsignacionCandidatoPuestoRepository
{
    public AsignacionCandidatoPuestoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AsignacionCandidatoPuesto>> GetByPartidoAsync(int partidoId)
    {
        // Trae toda la boleta electoral de un partido (sus candidatos asignados a presidencia, senaduría, etc.)
        return await _dbSet
            .Include(a => a.Candidato)
            .Include(a => a.PuestoElectivo)
            .Include(a => a.PartidoPolitico)
            .Where(a => a.PartidoPoliticoId == partidoId)
            .ToListAsync();
    }

    public async Task<bool> CandidatoTieneAsignacionEnPartidoAsync(int candidatoId, int partidoId)
    {
        // Evita que un mismo candidato sea postulado a dos puestos diferentes dentro de un mismo partido
        // Solo considera asignaciones activas (no eliminadas lógicamente)
        return await _dbSet.AnyAsync(a => a.CandidatoId == candidatoId && a.PartidoPoliticoId == partidoId && a.Activo);
    }

    public async Task<bool> PuestoTieneAsignacionEnPartidoAsync(int puestoId, int partidoId)
    {
        // Evita que un partido registre a dos candidatos diferentes para el mismo puesto (ej. Dos presidentes)
        // Solo considera asignaciones activas (no eliminadas lógicamente)
        return await _dbSet.AnyAsync(a => a.PuestoElectivoId == puestoId && a.PartidoPoliticoId == partidoId && a.Activo);
    }

    public async Task<bool> ExistenAsignacionesAliadasPorAlianzaAsync(int partidoAId, int partidoBId)
    {
        //  Para que un partido rompa una alianza, primero debemos validar que no tengan 
        // candidatos mezclados. Buscamos si hay un candidato original del Partido A corriendo en la 
        // boleta del Partido B (o viceversa) y que esté marcado como 'EsAliado'.
        return await _dbSet.AnyAsync(a => a.EsAliado &&
            ((a.Candidato.PartidoPoliticoId == partidoAId && a.PartidoPoliticoId == partidoBId) ||
             (a.Candidato.PartidoPoliticoId == partidoBId && a.PartidoPoliticoId == partidoAId)));
    }
}