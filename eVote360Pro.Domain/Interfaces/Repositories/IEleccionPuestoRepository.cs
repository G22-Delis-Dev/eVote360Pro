using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IEleccionPuestoRepository : IRepository<EleccionPuesto>
{
    Task<IEnumerable<EleccionPuesto>> GetByEleccionAsync(int eleccionId);
    Task<bool> ExistePuestoEnEleccionAsync(int eleccionId, int puestoId);
}