using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Votacion;

public class ValidacionIdentidadViewModel
{
    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [Display(Name = "Número de Identidad (Cédula)")]
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Cedula { get => NumeroDocumento; set => NumeroDocumento = value; }
}