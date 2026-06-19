namespace eVote360Pro.Application.ViewModels.Votacion;

public class ConfirmacionVotoViewModel
{
    public string MensajeExito { get; set; } = "Su voto ha sido procesado de manera totalmente anónima y segura";
    public DateTime FechaParticipacion { get; set; }
    public IEnumerable<ResumenSeleccionViewModel> VotosSeleccionados { get; set; } = [];
}

public class ResumenSeleccionViewModel
{
    public string PuestoNombre { get; set; } = string.Empty;
    public string? CandidatoNombreCompleto { get; set; }
    public string? FotoCandidato { get; set; }
    public string? PartidoNombre { get; set; }
    public string? PartidoSiglas { get; set; }
    public string? LogoPartido { get; set; }
}