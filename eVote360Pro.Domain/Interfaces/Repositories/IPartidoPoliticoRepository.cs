using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IPartidoPoliticoRepository : IRepository<PartidoPolitico>
{
    Task<bool> ExisteSiglasAsync(string siglas, int? excludeId = null);
    Task<bool> TieneCandidatosActivosAsync(int partidoId);
    Task<bool> TieneDirigenteAsignadoAsync(int partidoId);
    Task<bool> ParticipóEnEleccionAsync(int partidoId);
    Task<IEnumerable<PartidoPolitico>> GetActivosDisponiblesParaAsignacionAsync();
}