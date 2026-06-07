namespace eVote360Pro.Domain.Entities;

public class AsignacionCandidatoPuesto : BaseEntity
{
    public int CandidatoId { get; set; }
    public Candidato Candidato { get; set; } = null!;

    public int PuestoElectivoId { get; set; }
    public PuestoElectivo PuestoElectivo { get; set; } = null!;
    public int PartidoPoliticoId { get; set; }
    public PartidoPolitico PartidoPolitico { get; set; } = null!;
    public bool EsAliado { get; set; } = false;
}