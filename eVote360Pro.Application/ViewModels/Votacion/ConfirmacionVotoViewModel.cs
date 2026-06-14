namespace eVote360Pro.Application.ViewModels.Votacion;

public class ConfirmacionVotoViewModel
{
    public string MensajeExito { get; set; } = "Su voto ha sido procesado de manera totalmente anónima y segura";
    public DateTime FechaParticipacion { get; set; }
}