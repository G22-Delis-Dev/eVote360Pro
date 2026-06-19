using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IAlianzaPoliticaRepository : IRepository<AlianzaPolitica>
{
    Task<IEnumerable<AlianzaPolitica>> GetPendientesByReceptorAsync(int partidoReceptorId);
    Task<IEnumerable<AlianzaPolitica>> GetSolicitudesRealizadasAsync(int partidoSolicitanteId);
    Task<IEnumerable<AlianzaPolitica>> GetAlianzasVigentesAsync(int partidoId);
    Task<IEnumerable<AlianzaPolitica>> GetPorPartidoConNombresAsync(int partidoId);
    Task<bool> ExisteSolicitudPendienteAsync(int partidoAId, int partidoBId);
    Task<bool> ExisteAlianzaVigenteAsync(int partidoAId, int partidoBId);
    Task EliminarFisicoAsync(AlianzaPolitica alianza);
}