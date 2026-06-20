namespace eVote360Pro.Application.ViewModels.Votacion;

public class InicioVotacionViewModel
{
    public int EleccionId { get; set; }
    public string EleccionNombre { get; set; } = string.Empty;
    public string NombreEleccion { get => EleccionNombre; set => EleccionNombre = value; }
    public DateTime FechaRealizacion { get; set; }
    public bool HayEleccionActiva { get; set; } = true;
    public bool YaVoto { get; set; } = false;
}