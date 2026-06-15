using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Alianzas;

public class AlianzaCreateViewModel
{
    // El PartidoSolicitanteId ya no es seleccionable, se toma automáticamente del dirigente autenticado

    [Required(ErrorMessage = "Debe seleccionar el partido político receptor.")]
    [Display(Name = "Partido Receptor")]
    public int PartidoReceptorId { get; set; }
}