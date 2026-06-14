namespace eVote360Pro.Application.DTOs;

public class AsignacionCandidatoPuestoDto
{
    public int Id { get; set; }

    public int CandidatoId { get; set; }
    public string? CandidatoNombreCompleto { get; set; }

    public int PuestoElectivoId { get; set; }
    public string? PuestoNombre { get; set; }

    public int PartidoPoliticoId { get; set; }
    public string? PartidoNombre { get; set; }

    public bool EsAliado { get; set; }
}