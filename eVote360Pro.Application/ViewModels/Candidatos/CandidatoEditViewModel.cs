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
    public string NumeroDocumento { get; set; } = string.Empty;
    public bool DocumentoEsEditable { get; set; }

    [Display(Name = "Nueva Foto del Candidato (Opcional)")]
    public IFormFile? NuevaFoto { get; set; }
    public IFormFile? FotoArchivo { get => NuevaFoto; set => NuevaFoto = value; }

    public string FotoActual { get; set; } = string.Empty;
    public string FotoUrlExistente { get => FotoActual; set => FotoActual = value; }
    
    public bool Activo { get; set; }
}