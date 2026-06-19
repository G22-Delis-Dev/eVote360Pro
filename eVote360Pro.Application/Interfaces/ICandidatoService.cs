using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface ICandidatoService : IGenericService<CandidatoDto>
{
    Task<IEnumerable<CandidatoDto>> ObtenerPorPartidoAsync(int partidoId);
    Task<IEnumerable<CandidatoDto>> ObtenerAliadosPorPartidoAsync(int partidoId);
    Task CambiarEstadoAsync(int id);
}