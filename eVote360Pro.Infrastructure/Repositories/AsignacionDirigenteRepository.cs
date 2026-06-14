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
        // Buscamos si el usuario dirige algún partido y traemos la info de ese partido.
        return await _dbSet
            .Include(a => a.PartidoPolitico)
            .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId);
    }

    public async Task<AsignacionDirigente?> GetByPartidoAsync(int partidoId)
    {
        // Buscamos quién es el dirigente actual de un partido y traemos su información de Usuario (Nombre, Apellido).
        return await _dbSet
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.PartidoPoliticoId == partidoId);
    }
}