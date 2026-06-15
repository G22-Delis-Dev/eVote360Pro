using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Candidatos;

public class CandidatoEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    // El PartidoPoliticoId ya no es seleccionable, se toma automáticamente del dirigente autenticado

    [Display(Name = "Nueva Foto del Candidato (Opcional)")]
    public IFormFile? FotoArchivo { get; set; }

    public string FotoUrlExistente { get; set; } = string.Empty;
}