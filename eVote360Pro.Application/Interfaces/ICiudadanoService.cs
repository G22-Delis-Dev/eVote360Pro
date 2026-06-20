using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface ICiudadanoService : IGenericService<CiudadanoDto>
{
    // Solo definimos métodos que NO están en IGenericService
    Task<IEnumerable<CiudadanoDto>> ObtenerListaAsync(string? filtro = null);
    Task CambiarEstadoAsync(int id);
}