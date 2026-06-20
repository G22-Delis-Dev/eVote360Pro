using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Alianzas;

public class AlianzaCreateViewModel
{
    // El PartidoSolicitanteId ya no es seleccionable, se toma automáticamente del dirigente autenticado

    [Required(ErrorMessage = "Debe seleccionar el partido político receptor.")]
    [Display(Name = "Partido Receptor")]
    public int PartidoReceptorId { get; set; }

    public int PartidoPrincipalId { get; set; }
    public string PartidoPrincipalNombre { get; set; } = string.Empty;
    public int PuestoId { get; set; }
    public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? PuestosDisponibles { get; set; }
    public int PartidoAliadoId { get; set; }
    public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? PartidosAliadosDisponibles { get; set; }
}