namespace eVote360Pro.Domain.Entities;

public class PartidoPolitico : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Siglas { get; set; } = string.Empty;
    public string LogoRuta { get; set; } = string.Empty;

    // Navegación
    public AsignacionDirigente? AsignacionDirigente { get; set; }
    public ICollection<Candidato> Candidatos { get; set; } = [];
    public ICollection<AlianzaPolitica> AlianzasComoSolicitante { get; set; } = [];
    public ICollection<AlianzaPolitica> AlianzasComoReceptor { get; set; } = [];
    public ICollection<AsignacionCandidatoPuesto> AsignacionesCandidatos { get; set; } = [];
}