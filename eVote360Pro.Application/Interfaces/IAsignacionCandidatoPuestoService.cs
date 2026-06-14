using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IAsignacionCandidatoPuestoService
{
    Task<IEnumerable<AsignacionCandidatoPuestoDto>> ObtenerTodasAsync();
    Task<AsignacionCandidatoPuestoDto?> ObtenerPorIdAsync(int id);
    Task<AsignacionCandidatoPuestoDto> CrearAsync(AsignacionCandidatoPuestoDto dto);
    Task ActualizarAsync(int id, AsignacionCandidatoPuestoDto dto);
    Task EliminarAsync(int id);
}