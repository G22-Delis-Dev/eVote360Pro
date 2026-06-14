using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface ICandidatoService
{
    Task<IEnumerable<CandidatoDto>> ObtenerTodosAsync();
    Task<CandidatoDto?> ObtenerPorIdAsync(int id);
    // Para los dropdowns o vistas filtradas
    Task<IEnumerable<CandidatoDto>> ObtenerPorPartidoAsync(int partidoId);

    Task<CandidatoDto> CrearAsync(CandidatoDto candidatoDto);
    Task ActualizarAsync(int id, CandidatoDto candidatoDto);
    Task EliminarAsync(int id);
    Task CambiarEstadoAsync(int id); // Para activar/inactivar
}