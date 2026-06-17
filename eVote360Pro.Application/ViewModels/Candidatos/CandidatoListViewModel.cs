namespace eVote360Pro.Application.ViewModels.Candidatos;

public class CandidatoListViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string PartidoPoliticoNombre { get; set; } = string.Empty;
    public string FotoUrl { get; set; } = string.Empty;
    public bool Activo { get; set; }
    
    // Propiedades para la vista
    public string Filtro { get; set; } = string.Empty;
    public string NombrePuesto { get; set; } = string.Empty;
    public IEnumerable<CandidatoListViewModel> Candidatos { get; set; } = [];
    public string NumeroDocumento { get; set; } = string.Empty;
    public IEnumerable<string> PuestosAsignados { get; set; } = [];
    public bool EnEleccionActiva { get; set; }
}