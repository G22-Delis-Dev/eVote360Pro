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

    [Display(Name = "Foto del Candidato")]
    public IFormFile? FotoArchivo { get; set; }

    public string FotoUrlExistente { get; set; } = string.Empty;
}