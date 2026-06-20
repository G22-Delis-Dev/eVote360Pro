namespace eVote360Pro.Application.Interfaces;
public interface IGenericService<TDto> where TDto : class
{
    Task<IEnumerable<TDto>> ObtenerTodosAsync();
    Task<TDto?> ObtenerPorIdAsync(int id);
    Task<TDto> CrearAsync(TDto dto);
    Task ActualizarAsync(int id, TDto dto);
    Task EliminarAsync(int id);
}
