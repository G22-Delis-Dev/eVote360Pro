using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IAsignacionDirigenteService : IGenericService<AsignacionDirigenteDto>
{
    // ObtenerListaAsync ya es equivalente a ObtenerTodosAsync de la interfaz genérica.
    // Si prefieres mantener tu nombre específico, puedes dejarlo así:
    Task<IEnumerable<AsignacionDirigenteDto>> ObtenerListaAsync();

    // Métodos para llenar los SelectLists del formulario sin depender de ViewModels de la capa Web
    Task<IEnumerable<object>> ObtenerDirigentesDisponiblesAsync();
    Task<IEnumerable<object>> ObtenerPartidosDisponiblesAsync();
}