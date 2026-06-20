using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class PuestoElectivoRepository : GenericRepository<PuestoElectivo>, IPuestoElectivoRepository
{
    public PuestoElectivoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
    {
        // Limpiamos espacios y convertimos a mayúsculas para evitar duplicados como "Presidente" y " presidente "
        var nombreLimpio = nombre.Trim().ToUpper();

        if (excludeId.HasValue)
        {
            // Para la edición: Verifica si existe el nombre en OTRO puesto electivo
            return await _dbSet.AnyAsync(p => p.Nombre.Trim().ToUpper() == nombreLimpio && p.Id != excludeId.Value);
        }

        // Para la creación: Verifica que el nombre no se repita
        return await _dbSet.AnyAsync(p => p.Nombre.Trim().ToUpper() == nombreLimpio);
    }

    public async Task<bool> ParticipóEnEleccionAsync(int puestoId)
    {
        // Un puesto electivo se considera utilizado si ya fue incluido en alguna elección 
        // (EleccionPuestos) o si ya tiene candidatos compitiendo por él (AsignacionesCandidatos).
        return await _dbSet.AnyAsync(p => p.Id == puestoId &&
                                         (p.EleccionPuestos.Any() || p.AsignacionesCandidatos.Any()));
    }
}