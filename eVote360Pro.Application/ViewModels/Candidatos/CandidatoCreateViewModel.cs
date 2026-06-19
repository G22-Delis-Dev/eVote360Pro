using Microsoft.AspNetCore.Http; // Aquí sí es correcto usarlo
using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Candidatos; // CAMBIA EL NAMESPACE

public class CandidatoCreateViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    [Display(Name = "Foto del Candidato")]
    [Required(ErrorMessage = "La foto del candidato es obligatoria")]
    public IFormFile? FotoFile { get; set; }
    public IFormFile? FotoArchivo { get => FotoFile; set => FotoFile = value; }

    public string FotoUrlExistente { get; set; } = string.Empty;

    public int CiudadanoId { get; set; }
    public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? CiudadanosDisponibles { get; set; }
}