using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Application.ViewModels.Elecciones;

public class EleccionCreateViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200)]
    [Display(Name = "Nombre de la elección")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de realización")]
    public DateTime FechaRealizacion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar al menos un puesto electivo.")]
    [Display(Name = "Puestos electivos")]
    public List<int> PuestosSeleccionados { get; set; } = [];

    public IEnumerable<SelectListItem> PuestosDisponibles { get; set; } = [];
}