using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IUsuarioService : IGenericService<UsuarioDto>
{
    // Métodos de negocio que devuelven DTOs
    Task<IEnumerable<UsuarioDto>> ObtenerListaAsync();

    // Operaciones de persistencia usando DTOs
    Task CrearAsync(UsuarioDto dto, string password);
    Task EditarAsync(UsuarioDto dto, string? nuevaPassword = null);
    Task ToggleActivoAsync(int id, int usuarioActualId);
    Task<UsuarioDto?> ValidarCredencialesAsync(string nombreUsuario, string password);
}