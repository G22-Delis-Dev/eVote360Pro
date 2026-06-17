namespace eVote360Pro.Application.ViewModels.Home;

public class HomeDirigenteViewModel
{
    public int TotalPartidosPoliticos { get; set; }
    public int TotalCandidatos { get; set; }
    public int TotalAlianzasPendientes { get; set; }
    public int TotalAsignaciones { get; set; }
    public int TotalEleccionesActivas { get; set; }

    public int TotalCandidatosPropios { get => TotalCandidatos; set => TotalCandidatos = value; }
    public int TotalAlianzasActivas { get; set; }
    public int TotalSolicitudesPendientes { get; set; }
    public int TotalCandidatosAsignadosPuesto { get; set; }
}