using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IPuestoElectivoService
{
    Task<IEnumerable<PuestoElectivoDto>> ObtenerTodosAsync();

    // Hay que agg aquí los métodos Create, Update y Delete cuando te toque este módulo, solo vree esto pa que mi service de AsignacionCandidato funcione.
}