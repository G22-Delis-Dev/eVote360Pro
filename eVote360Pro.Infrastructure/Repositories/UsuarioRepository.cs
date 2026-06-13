using Microsoft.EntityFrameworkCore;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums; 
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;

namespace eVote360Pro.Infrastructure.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
    {
        // Fundamental para el proceso de Login (Autenticación)
        return await _dbSet.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? excludeId = null)
    {
        // Valida que no se dupliquen los Usernames al crear o editar
        if (excludeId.HasValue)
        {
            return await _dbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario && u.Id != excludeId.Value);
        }

        return await _dbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<bool> ExisteCorreoElectronicoAsync(string correo, int? excludeId = null)
    {
        // Valida que no se dupliquen los correos al crear o editar
        if (excludeId.HasValue)
        {
            return await _dbSet.AnyAsync(u => u.CorreoElectronico == correo && u.Id != excludeId.Value);
        }

        return await _dbSet.AnyAsync(u => u.CorreoElectronico == correo);
    }

    public async Task<int> ContarAdministradoresActivosAsync()
    {
        // Regla de negocio crítica: Evitar que el sistema se quede sin administradores.
        // Si esto devuelve 1, la capa de Aplicación deberá bloquear la eliminación o desactivación de ese usuario.
        return await _dbSet.CountAsync(u => u.Activo && u.Rol == RolUsuario.Administrador);
    }

    public async Task<IEnumerable<Usuario>> GetDirigentesDisponiblesParaAsignacionAsync()
    {
        // Trae solo los usuarios activos, que tienen el rol correcto, 
        // y cuya propiedad de navegación "AsignacionDirigente" está vacía (es decir, no lideran ningún partido aún).
        return await _dbSet
            .Where(u => u.Activo &&
                        u.Rol == RolUsuario.DirigentePolitico &&
                        u.AsignacionDirigente == null)
            .ToListAsync();
    }
}