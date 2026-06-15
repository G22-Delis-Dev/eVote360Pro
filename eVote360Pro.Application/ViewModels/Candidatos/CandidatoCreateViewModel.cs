using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Candidatos;

public class CandidatoCreateViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    // El PartidoPoliticoId ya no es seleccionable, se toma automáticamente del dirigente autenticado

    // Propiedad para recibir el archivo físico de la vista
    [Display(Name = "Foto del Candidato")]
    public IFormFile? FotoArchivo { get; set; }

    public string FotoUrlExistente { get; set; } = string.Empty; // Para guardar la foto si estamos editando
}