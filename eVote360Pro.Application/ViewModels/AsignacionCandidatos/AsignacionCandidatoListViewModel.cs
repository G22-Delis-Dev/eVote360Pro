namespace eVote360Pro.Application.ViewModels.AsignacionCandidatos;

public class AsignacionCandidatoListViewModel
{
    public int Id { get; set; }
    public string CandidatoNombreCompleto { get; set; } = string.Empty;
    public string PuestoNombre { get; set; } = string.Empty;
    public string PartidoNombre { get; set; } = string.Empty;
    public bool EsAliado { get; set; }
}