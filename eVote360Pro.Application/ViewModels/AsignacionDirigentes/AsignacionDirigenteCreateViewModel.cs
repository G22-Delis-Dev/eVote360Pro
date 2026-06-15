using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Application.ViewModels.AsignacionDirigentes;

public class AsignacionDirigenteCreateViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un dirigente")]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un partido político")]
    public int PartidoPoliticoId { get; set; }

    // Listas para los Dropdowns en la vista
    public IEnumerable<SelectListItem>? DirigentesDisponibles { get; set; }
    public IEnumerable<SelectListItem>? PartidosDisponibles { get; set; }
}