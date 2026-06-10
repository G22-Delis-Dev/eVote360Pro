using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IPuestoElectivoRepository : IRepository<PuestoElectivo>
{
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
    Task<bool> ParticipóEnEleccionAsync(int puestoId);
}