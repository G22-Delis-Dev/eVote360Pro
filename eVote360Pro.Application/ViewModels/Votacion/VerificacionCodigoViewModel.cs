using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Application.ViewModels.Votacion;

public class VerificacionCodigoViewModel
{
    public int CiudadanoId { get; set; }
    public int EleccionId { get; set; }

    [Required(ErrorMessage = "El código de verificación es obligatorio.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener exactamente 6 dígitos.")]
    [Display(Name = "Código Secreto")]
    public string Codigo { get; set; } = string.Empty;
    public string CodigoVerificacion { get => Codigo; set => Codigo = value; }

    public string CorreoElectronicoOculto { get; set; } = string.Empty;
    public int VotanteId { get => CiudadanoId; set => CiudadanoId = value; }
}