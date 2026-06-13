using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class CodigoVerificacionRepository : GenericRepository<CodigoVerificacion>, ICodigoVerificacionRepository
{
    public CodigoVerificacionRepository(AppDbContext context) : base(context) { }

    public async Task<CodigoVerificacion?> GetCodigoVigenteAsync(int ciudadanoId, int eleccionId, string codigo)
    {
        var ahora = DateTime.UtcNow;
        return await _dbSet
            .FirstOrDefaultAsync(c => c.CiudadanoId == ciudadanoId
                                  && c.EleccionId == eleccionId
                                  && c.Codigo == codigo
                                  && !c.Utilizado
                                  && c.FechaExpiracion > ahora);
    }

    public async Task InvalidarCodigosAnterioresAsync(int ciudadanoId, int eleccionId)
    {
        var codigos = await _dbSet
            .Where(c => c.CiudadanoId == ciudadanoId && c.EleccionId == eleccionId && !c.Utilizado)
            .ToListAsync();

        foreach (var codigo in codigos)
        {
            codigo.Utilizado = true;
        }
        await _context.SaveChangesAsync();
    }
}