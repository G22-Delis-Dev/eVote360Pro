using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.AsignacionCandidatos;

public class AsignacionCandidatoCreateViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un candidato.")]
    [Display(Name = "Candidato")]
    public int CandidatoId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un puesto electivo.")]
    [Display(Name = "Puesto Electivo")]
    public int PuestoElectivoId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el partido político.")]
    [Display(Name = "Partido Político")]
    public int PartidoPoliticoId { get; set; }

    [Display(Name = "¿Va como aliado?")] 
    public bool EsAliado { get; set; }
}