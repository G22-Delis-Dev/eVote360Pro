namespace eVote360Pro.Application.ViewModels.Candidatos;

public class CandidatoListViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string PartidoPoliticoNombre { get; set; } = string.Empty;
    public string FotoUrl { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
}