using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IParticipacionElectoralRepository : IRepository<ParticipacionElectoral>
{
    Task<bool> CiudadanoYaVotóAsync(int ciudadanoId, int eleccionId);
    Task<int> ContarParticipantesPorEleccionAsync(int eleccionId);
}