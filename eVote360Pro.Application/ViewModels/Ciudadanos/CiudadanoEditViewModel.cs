using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Ciudadanos;

public class CiudadanoEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento es obligatorio")]
    public string NumeroDocumento { get; set; } = string.Empty;

    public bool Activo { get; set; }

    // Campo específico para la vista (no es parte del DTO)
    public bool DocumentoEsEditable { get; set; }
}