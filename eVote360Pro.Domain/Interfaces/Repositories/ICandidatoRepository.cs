using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface ICandidatoRepository : IRepository<Candidato>
{
    Task<IEnumerable<Candidato>> GetActivosByPartidoAsync(int partidoId);
    Task<bool> EstaAsignadoAPuestoAsync(int candidatoId);
    Task<bool> ParticipóEnEleccionAsync(int candidatoId);
}