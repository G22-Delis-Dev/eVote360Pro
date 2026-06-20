using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;
namespace eVote360Pro.Infrastructure.Repositories
{
    public class PartidoPoliticoRepository : GenericRepository<PartidoPolitico>, IPartidoPoliticoRepository
    {
        public PartidoPoliticoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExisteSiglasAsync(string siglas, int? excludeId = null)
        {
            // El requerimiento exige que las siglas sean únicas en todo el sistema
                        // Usamos ToUpper() y Trim() por seguridad, aunque SQL Server suele ser Case-Insensitive.
            var siglasLimpia = siglas.Trim().ToUpper();

            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(p => p.Siglas.Trim().ToUpper() == siglasLimpia && p.Id != excludeId.Value);
            }

            // Para creación
            return await _dbSet.AnyAsync(p => p.Siglas.Trim().ToUpper() == siglasLimpia);
        }

        public async Task<bool> TieneCandidatosActivosAsync(int partidoId)
        {
                     // Validamos si no se puede desactivar porque tiene candidatos activos
                        // Truco de Senior: Usar la propiedad de navegación ".Candidatos" salva de adivinar el nombre de la llave foránea
            return await _dbSet.AnyAsync(p => p.Id == partidoId && p.Candidatos.Any(c => c.Activo));
        }

        public async Task<bool> TieneDirigenteAsignadoAsync(int partidoId)
        {
           // Validamos si el partido no se puede desactivar porque ya tiene un dirigente asignado
            return await _dbSet.AnyAsync(p => p.Id == partidoId && p.AsignacionDirigente != null);
        }

        public async Task<IEnumerable<PartidoPolitico>> GetActivosDisponiblesParaAsignacionAsync()
        {
            // Carga el select para asignar dirigentes: Solo partidos activos y sin dirigente
            return await _dbSet
                .Where(p => p.Activo && p.AsignacionDirigente == null)
                .ToListAsync();
        }

        public async Task<bool> ParticipoEnEleccionAsync(int partidoId)
        {

            // Para bloquear campos críticos si el partido ya participó en una elección
            // Se considera que participó si alguno de sus candidatos tiene una asignación a un puesto en una elección.
            return await _dbSet.AnyAsync(p => p.Id == partidoId &&
                                              p.Candidatos.Any(c => c.AsignacionesPuestos.Any()));
        }
    }
}