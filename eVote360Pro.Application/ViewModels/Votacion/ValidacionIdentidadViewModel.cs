using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Votacion;

public class ValidacionIdentidadViewModel
{
    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [Display(Name = "Número de Identidad (Cédula)")]
    public string Cedula { get; set; } = string.Empty;
}