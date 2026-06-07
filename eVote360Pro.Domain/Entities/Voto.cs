namespace eVote360Pro.Domain.Entities;

public class Voto : BaseEntity
{
    public int EleccionId { get; set; }
    public Eleccion Eleccion { get; set; } = null!;

    public int PuestoElectivoId { get; set; }
    public PuestoElectivo PuestoElectivo { get; set; } = null!;
    public int? CandidatoId { get; set; }
    public Candidato? Candidato { get; set; }
    public int? PartidoPoliticoId { get; set; }
    public PartidoPolitico? PartidoPolitico { get; set; }
}