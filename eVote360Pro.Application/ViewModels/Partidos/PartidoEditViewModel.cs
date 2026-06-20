using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Application.ViewModels.Partidos;

public class PartidoEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Las siglas son obligatorias")]
    public string Siglas { get; set; } = string.Empty;

    public string? LogoActual { get; set; }
    public string? LogoUrlActual { get => LogoActual; set => LogoActual = value; }
    public string? LogoActualRuta { get => LogoActual; set => LogoActual = value; }

    public IFormFile? NuevoLogoArchivo { get; set; }
    public IFormFile? LogoFile { get => NuevoLogoArchivo; set => NuevoLogoArchivo = value; }

    public bool Activo { get; set; }

    // Campo para lógica de UI en la vista
    public bool CamposCriticosEditables { get; set; }
}