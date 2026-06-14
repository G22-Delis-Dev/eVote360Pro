using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Alianzas;

public class AlianzaCreateViewModel
{
    [Required(ErrorMessage = "Debe seleccionar el partido político que solicita la alianza.")]
    [Display(Name = "Partido Solicitante")]
    public int PartidoSolicitanteId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el partido político receptor.")]
    [Display(Name = "Partido Receptor")]
    public int PartidoReceptorId { get; set; }
}