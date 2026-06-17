namespace eVote360Pro.Application.ViewModels.Partidos;

public class PartidoListViewModel
{
    public IEnumerable<PartidoItemViewModel> Partidos { get; set; } = new List<PartidoItemViewModel>();
}

public class PartidoItemViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Siglas { get; set; } = string.Empty;
    public string? LogoRuta { get; set; }
    public string? LogoUrl { get; set; }
    public bool Activo { get; set; }
    public bool EnEleccionActiva { get; set; }
}