using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class AlianzaPoliticaRepository : GenericRepository<AlianzaPolitica>, IAlianzaPoliticaRepository
{
    public AlianzaPoliticaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AlianzaPolitica>> GetPendientesByReceptorAsync(int partidoReceptorId)
    {
        // Trae la bandeja de entrada de un partido: las solicitudes que otros le han enviado y aún no responde.
        return await _dbSet
            .Include(a => a.PartidoSolicitante) // Incluimos esto para poder mostrar "El Partido X te envió una solicitud"
            .Where(a => a.PartidoReceptorId == partidoReceptorId && a.Estado == EstadoAlianza.Pendiente)
            .ToListAsync();
    }

    public async Task<IEnumerable<AlianzaPolitica>> GetSolicitudesRealizadasAsync(int partidoSolicitanteId)
    {
        // Trae la bandeja de salida: las solicitudes que este partido envió a otros.
        return await _dbSet
            .Include(a => a.PartidoReceptor)
            .Where(a => a.PartidoSolicitanteId == partidoSolicitanteId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AlianzaPolitica>> GetAlianzasVigentesAsync(int partidoId)
    {
        // Trae las alianzas activas sin importar si este partido fue el que la pidió o el que la aceptó.
        return await _dbSet
            .Include(a => a.PartidoSolicitante)
            .Include(a => a.PartidoReceptor)
            .Where(a => (a.PartidoSolicitanteId == partidoId || a.PartidoReceptorId == partidoId)
                     && a.Estado == EstadoAlianza.Aceptada) // <-- Cambia "Aprobada" si Delis usó otra palabra en el Enum
            .ToListAsync();
    }

    public async Task<bool> ExisteSolicitudPendienteAsync(int partidoAId, int partidoBId)
    {
        // Valida que no envíen una solicitud si ya hay una pendiente entre ellos dos (en cualquier dirección)
        return await _dbSet.AnyAsync(a =>
            ((a.PartidoSolicitanteId == partidoAId && a.PartidoReceptorId == partidoBId) ||
             (a.PartidoSolicitanteId == partidoBId && a.PartidoReceptorId == partidoAId)) &&
            a.Estado == EstadoAlianza.Pendiente);
    }

    public async Task<bool> ExisteAlianzaVigenteAsync(int partidoAId, int partidoBId)
    {
        // Valida que no se alien dos veces los mismos partidos
        return await _dbSet.AnyAsync(a =>
            ((a.PartidoSolicitanteId == partidoAId && a.PartidoReceptorId == partidoBId) ||
             (a.PartidoSolicitanteId == partidoBId && a.PartidoReceptorId == partidoAId)) &&
            a.Estado == EstadoAlianza.Aceptada);
    }
}