using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class EleccionRepository : GenericRepository<Eleccion>, IEleccionRepository
{
    public EleccionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Eleccion?> GetEleccionActivaAsync()
    {
        // Solo puede haber una elección activa a la vez. Fundamental para saber a dónde van los votos en tiempo real.
        return await _dbSet.FirstOrDefaultAsync(e => e.Estado == EstadoEleccion.Activa);
    }

    public async Task<bool> ExisteEleccionActivaAsync()
    {
        // Usado antes de abrir una nueva elección para evitar que hayan dos elecciones abiertas al mismo tiempo.
        return await _dbSet.AnyAsync(e => e.Estado == EstadoEleccion.Activa);
    }

    public async Task<IEnumerable<Eleccion>> GetOrdenadaPorFechaDescAsync()
    {
        // Trae todo el historial de elecciones, de la más reciente a la más antigua (Para el panel de administración).
        return await _dbSet
            .OrderByDescending(e => e.FechaRealizacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Eleccion>> GetByAnioAsync(int anio)
    {
        // Para filtrar elecciones por un año específico sacando el año directo de la fecha.
        return await _dbSet
            .Where(e => e.FechaRealizacion.Year == anio)
            .ToListAsync();
    }
}