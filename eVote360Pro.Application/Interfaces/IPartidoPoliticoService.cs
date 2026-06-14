using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IPartidoPoliticoService
{
    // El método el dropdown
    Task<IEnumerable<PartidoPoliticoDto>> ObtenerTodosAsync();

    // Los demás métodos se implementaran despues, estos son solo para terminar el servicio de Candidato
    Task<PartidoPoliticoDto?> ObtenerPorIdAsync(int id);
    Task<PartidoPoliticoDto> CrearAsync(PartidoPoliticoDto partidoDto);
    Task ActualizarAsync(int id, PartidoPoliticoDto partidoDto);
    Task EliminarAsync(int id);
    Task CambiarEstadoAsync(int id);
}