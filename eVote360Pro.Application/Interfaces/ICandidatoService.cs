using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface ICandidatoService : IGenericService<CandidatoDto>
{
    // Para los dropdowns o vistas filtradas
    Task<IEnumerable<CandidatoDto>> ObtenerPorPartidoAsync(int partidoId);
    Task CambiarEstadoAsync(int id); // Para activar/inactivar
}