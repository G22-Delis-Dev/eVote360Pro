using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IAsignacionCandidatoPuestoRepository : IRepository<AsignacionCandidatoPuesto>
{
    Task<IEnumerable<AsignacionCandidatoPuesto>> GetByPartidoAsync(int partidoId);
    Task<bool> CandidatoTieneAsignacionEnPartidoAsync(int candidatoId, int partidoId);
    Task<bool> PuestoTieneAsignacionEnPartidoAsync(int puestoId, int partidoId);
    Task<bool> ExistenAsignacionesAliadasPorAlianzaAsync(int partidoAId, int partidoBId);
}