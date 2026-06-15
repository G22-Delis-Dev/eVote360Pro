namespace eVote360Pro.Application.ViewModels.AsignacionDirigentes;

public class AsignacionDirigenteListViewModel
{
    public IEnumerable<AsignacionDirigenteItemViewModel> Asignaciones { get; set; } = new List<AsignacionDirigenteItemViewModel>();
    public bool HayEleccionActiva { get; set; }
}

public class AsignacionDirigenteItemViewModel
{
    public int Id { get; set; }
    public string NombreDirigente { get; set; } = string.Empty;
    public string NombrePartido { get; set; } = string.Empty;
    public string SiglaPartido { get; set; } = string.Empty;
}