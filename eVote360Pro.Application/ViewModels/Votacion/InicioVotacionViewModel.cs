namespace eVote360Pro.Application.ViewModels.Votacion;

public class InicioVotacionViewModel
{
    public int EleccionId { get; set; }
    public string EleccionNombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
}