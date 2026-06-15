using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IPartidoPoliticoService : IGenericService<PartidoPoliticoDto>
{
    // Métodos específicos de negocio que retornan DTOs
    Task<IEnumerable<PartidoPoliticoDto>> ObtenerActivosAsync();

    // Operaciones usando DTOs (el controlador mapea de/hacia ViewModel)
    Task CrearAsync(PartidoPoliticoDto dto, string rutaLogo);
    Task EditarAsync(PartidoPoliticoDto dto, string? rutaLogo);
    Task CambiarEstadoAsync(int id);
}