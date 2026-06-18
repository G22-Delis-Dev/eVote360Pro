using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IAsignacionDirigenteService : IGenericService<AsignacionDirigenteDto>
{
    // ObtenerListaAsync ya es equivalente a ObtenerTodosAsync de la interfaz genérica.
    Task<IEnumerable<AsignacionDirigenteDto>> ObtenerListaAsync();

    // Métodos para llenar los SelectLists del formulario — retornan tipos concretos, no object
    Task<IEnumerable<DropdownItemDto>> ObtenerDirigentesDisponiblesAsync();
    Task<IEnumerable<DropdownItemDto>> ObtenerPartidosDisponiblesAsync();
}