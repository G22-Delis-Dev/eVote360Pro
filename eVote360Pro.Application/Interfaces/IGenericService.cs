namespace eVote360Pro.Application.Interfaces;

/// <summary>
/// Interfaz genérica que define las operaciones CRUD estándar para cualquier servicio.
/// Los servicios específicos heredan de esta interfaz y solo agregan sus métodos propios.
/// </summary>
public interface IGenericService<TDto> where TDto : class
{
    Task<IEnumerable<TDto>> ObtenerTodosAsync();
    Task<TDto?> ObtenerPorIdAsync(int id);
    Task<TDto> CrearAsync(TDto dto);
    Task ActualizarAsync(int id, TDto dto);
    Task EliminarAsync(int id);
}
