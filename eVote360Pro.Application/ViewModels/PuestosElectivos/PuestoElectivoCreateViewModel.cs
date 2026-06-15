using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.PuestosElectivos;

public class PuestoElectivoCreateViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string Descripcion { get; set; } = string.Empty;
}