namespace eVote360Pro.Application.ViewModels.AsignacionDirigentes;

public class AsignacionDirigenteListViewModel
{
    public IEnumerable<AsignacionDirigenteItemViewModel> Asignaciones { get; set; } = new List<AsignacionDirigenteItemViewModel>();
    public bool HayEleccionActiva { get; set; }
    public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? PartidosDisponibles { get; set; }
    public int? PartidoFiltroId { get; set; }
}

public class AsignacionDirigenteItemViewModel
{
    public int Id { get; set; }
    public string NombreDirigente { get; set; } = string.Empty;
    public string NombrePartido { get; set; } = string.Empty;
    public string SiglaPartido { get; set; } = string.Empty;
    public string PartidoSiglas { get => SiglaPartido; set => SiglaPartido = value; }

    public string UsuarioDirigente { get => NombreDirigente; set => NombreDirigente = value; }
    public string PartidoNombre { get => NombrePartido; set => NombrePartido = value; }
    public DateTime FechaAsignacion { get; set; }
}