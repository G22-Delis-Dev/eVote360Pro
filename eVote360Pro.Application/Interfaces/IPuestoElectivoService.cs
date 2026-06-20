using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IPuestoElectivoService : IGenericService<PuestoElectivoDto>
{
    Task<IEnumerable<PuestoElectivoDto>> ObtenerActivosAsync();
    Task CambiarEstadoAsync(int id);
    Task<bool> ParticipoEnEleccionAsync(int id);
}