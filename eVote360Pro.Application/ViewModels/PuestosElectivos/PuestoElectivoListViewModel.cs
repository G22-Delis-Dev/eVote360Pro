namespace eVote360Pro.Application.ViewModels.PuestosElectivos;

public class PuestoElectivoListViewModel
{
    public IEnumerable<PuestoElectivoItemViewModel> Puestos { get; set; } = new List<PuestoElectivoItemViewModel>();
}

public class PuestoElectivoItemViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public bool EnEleccionActiva { get; set; }
}