using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? excludeId = null);
    Task<bool> ExisteCorreoElectronicoAsync(string correo, int? excludeId = null);
    Task<int> ContarAdministradoresActivosAsync();
    Task<IEnumerable<Usuario>> GetDirigentesDisponiblesParaAsignacionAsync();
}