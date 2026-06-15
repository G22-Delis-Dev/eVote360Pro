namespace eVote360Pro.Application.ViewModels.Ciudadanos;

public class CiudadanoListViewModel
{
    public IEnumerable<CiudadanoItemViewModel> Ciudadanos { get; set; } = new List<CiudadanoItemViewModel>();
    public string? Filtro { get; set; }
}

public class CiudadanoItemViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public bool Activo { get; set; }
}