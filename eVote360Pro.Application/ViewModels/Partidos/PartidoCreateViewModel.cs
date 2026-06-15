using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Application.ViewModels.Partidos;

public class PartidoCreateViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Las siglas son obligatorias")]
    [StringLength(10, ErrorMessage = "Máximo 10 caracteres")]
    public string Siglas { get; set; } = string.Empty;

    [Required(ErrorMessage = "El logo es obligatorio")]
    public IFormFile LogoArchivo { get; set; } = null!;
}