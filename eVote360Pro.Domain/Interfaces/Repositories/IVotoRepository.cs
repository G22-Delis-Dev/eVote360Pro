using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IVotoRepository : IRepository<Voto>
{
    Task<IEnumerable<Voto>> GetByEleccionYPuestoAsync(int eleccionId, int puestoId);
    Task<int> ContarVotosPorCandidatoAsync(int eleccionId, int puestoId, int? candidatoId);
}