using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Application.ViewModels.Usuarios;

public class UsuarioEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string? NuevaPassword { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un rol")]
    public int Rol { get; set; }

    public bool Activo { get; set; }
    public IEnumerable<SelectListItem>? RolesDisponibles { get; set; }
}