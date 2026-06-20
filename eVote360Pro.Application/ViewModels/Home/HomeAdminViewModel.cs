namespace eVote360Pro.Application.ViewModels.Home;

public class HomeAdminViewModel
{
    public int TotalUsuarios { get; set; }
    public int TotalCiudadanos { get; set; }
    public int TotalPartidosPoliticos { get; set; }
    public int TotalPuestosElectivos { get; set; }
    public int TotalAsignacionesDirigentes { get; set; }

    // Estadísticas de elecciones
    public int TotalElecciones { get; set; }
    public int EleccionesActivas { get; set; }
    public int EleccionesFinalizadas { get; set; }
    public int EleccionesPendientes { get; set; }

    // Resumen del año actual
    public int AnioConsulta { get; set; }
    public IEnumerable<ResumenEleccionItemViewModel> ResumenElecciones { get; set; } = [];
}

public class ResumenEleccionItemViewModel
{
    public int Id { get; set; }
    public string NombreEleccion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int TotalPartidos { get; set; }
    public int TotalCandidatos { get; set; }
    public int TotalVotantes { get; set; }
}
