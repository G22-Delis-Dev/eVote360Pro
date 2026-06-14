namespace eVote360Pro.Application.DTOs;

public class VotoDto
{
    public int EleccionId { get; set; }
    public int PuestoElectivoId { get; set; }
    public int? CandidatoId { get; set; }
    public int? PartidoPoliticoId { get; set; }
}