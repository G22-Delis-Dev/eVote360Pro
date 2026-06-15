using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class AsignacionDirigenteRepository : GenericRepository<AsignacionDirigente>, IAsignacionDirigenteRepository
{
    public AsignacionDirigenteRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<AsignacionDirigente?> GetByUsuarioAsync(int usuarioId)
    {
        return await _dbSet
            .Include(a => a.PartidoPolitico)
            .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId);
    }

    public async Task<AsignacionDirigente?> GetByPartidoAsync(int partidoId)
    {
        return await _dbSet
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.PartidoPoliticoId == partidoId);
    }

    // Implementación de los nuevos métodos requeridos por la interfaz
    public async Task<bool> DirigenteTienePartidoAsync(int usuarioId)
    {
        return await _dbSet.AnyAsync(a => a.UsuarioId == usuarioId);
    }

    public async Task<bool> PartidoTieneDirigenteAsync(int partidoId)
    {
        return await _dbSet.AnyAsync(a => a.PartidoPoliticoId == partidoId);
    }
}