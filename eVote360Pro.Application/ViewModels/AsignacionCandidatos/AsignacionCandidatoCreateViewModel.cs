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
    public int PuestoId { get => PuestoElectivoId; set => PuestoElectivoId = value; }

    public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? PuestosDisponibles { get; set; }
    public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? CandidatosDisponibles { get; set; }

    // El PartidoPoliticoId ya no es seleccionable, se toma automáticamente del dirigente autenticado

    [Display(Name = "¿Va como aliado?")] 
    public bool EsAliado { get; set; }
}