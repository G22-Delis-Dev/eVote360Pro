using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Application.ViewModels.Votacion;

public class ValidacionOcrViewModel
{
    public int CiudadanoId { get; set; }
    public int EleccionId { get; set; }

    /// <summary>Número de cédula ingresado en el paso anterior, usado para comparar con el OCR.</summary>
    public string CedulaIngresada { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe subir una imagen de su cédula para validar su identidad.")]
    [Display(Name = "Imagen de la cédula")]
    public IFormFile? ImagenCedula { get; set; }
}
