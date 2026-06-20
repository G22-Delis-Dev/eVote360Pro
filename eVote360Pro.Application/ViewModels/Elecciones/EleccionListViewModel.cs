using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Application.ViewModels.Elecciones;

public class EleccionListViewModel
{
    public IEnumerable<EleccionItemViewModel> Elecciones { get; set; } = [];
    public bool HayEleccionActiva { get; set; }
    public string AnioFiltro { get; set; } = string.Empty;
    public string EstadoFiltro { get; set; } = string.Empty;
}

public class EleccionItemViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public EstadoEleccion Estado { get; set; }
    public string EstadoNombre => Estado switch
    {
        EstadoEleccion.Pendiente => "Pendiente",
        EstadoEleccion.Activa => "Activa",
        EstadoEleccion.Finalizada => "Finalizada",
        _ => string.Empty
    };
    public DateTime? FechaActivacion { get; set; }
    public DateTime? FechaFinalizacion { get; set; }

    public DateTime FechaEleccion => FechaRealizacion;
    public IEnumerable<dynamic> PuestosElectivos { get; set; } = [];
}