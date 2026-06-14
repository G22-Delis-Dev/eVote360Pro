namespace eVote360Pro.Application.ViewModels.Votacion;

public class BoletaElectoralViewModel
{
    public int CiudadanoId { get; set; }
    public int EleccionId { get; set; }

    // Almacena qué candidato/partido eligió para cada puesto electivo
    public List<SeleccionVotoViewModel> Selecciones { get; set; } = new();
}