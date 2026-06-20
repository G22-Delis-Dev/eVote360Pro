using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IAsignacionCandidatoPuestoService : IGenericService<AsignacionCandidatoPuestoDto>
{
    Task<IEnumerable<AsignacionCandidatoPuestoDto>> ObtenerPorPartidoAsync(int partidoId);
}