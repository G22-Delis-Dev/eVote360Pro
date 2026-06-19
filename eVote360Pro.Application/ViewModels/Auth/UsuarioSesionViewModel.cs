using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Application.ViewModels.Auth;

public class UsuarioSesionViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public int? PartidoId { get; set; }
}
