using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories
{
    public class CiudadanoRepository : GenericRepository<Ciudadano>, ICiudadanoRepository
    {
        public CiudadanoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Ciudadano?> GetByNumeroDocumentoAsync(string numeroDocumento)
    {
        // Se utiliza para iniciar el proceso de votación del elector
        return await _dbSet.FirstOrDefaultAsync(c => c.NumeroDocumento == numeroDocumento);
    }

    public async Task<bool> ExisteNumeroDocumentoAsync(string numeroDocumento, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            // Para la edición: Verifica si existe la cédula en OTRO ciudadano [cite: 642]
            return await _dbSet.AnyAsync(c => c.NumeroDocumento == numeroDocumento && c.Id != excludeId.Value);
        }
        
        // Para la creación: Verifica que la cédula no se repita en toda la tabla [cite: 603]
        return await _dbSet.AnyAsync(c => c.NumeroDocumento == numeroDocumento);
    }

    public async Task<bool> ExisteCorreoElectronicoAsync(string correo, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            // Para la edición: Valida que el correo sea único ignorando al usuario actual
            return await _dbSet.AnyAsync(c => c.CorreoElectronico == correo && c.Id != excludeId.Value);
        }

       // Para la creación: Valida unicidad en toda la tabla [cite: 601]
        return await _dbSet.AnyAsync(c => c.CorreoElectronico == correo);
    }

    public async Task<bool> ParticipóEnEleccionAsync(int ciudadanoId)
    {
        // Usamos la tabla conectada ParticipacionesElectorales para saber si hay algún registro histórico
        return await _context.ParticipacionesElectorales
            .AnyAsync(p => p.CiudadanoId == ciudadanoId); 
    }

    public async Task<bool> YaVotoEnEleccionAsync(int ciudadanoId, int eleccionId)
    {
        // Valida si el ciudadano ya ejerció su voto en la elección activa específica [cite: 51, 275]
        return await _context.ParticipacionesElectorales
            .AnyAsync(p => p.CiudadanoId == ciudadanoId && p.EleccionId == eleccionId);
    }
}
   }
