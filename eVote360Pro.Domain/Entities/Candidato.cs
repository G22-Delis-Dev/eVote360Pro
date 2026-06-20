using eVote360Pro.Domain.Entities;

public class Candidato : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? FotoRuta { get; set; }
    public int PartidoPoliticoId { get; set; }
    public PartidoPolitico PartidoPolitico { get; set; } = null!;
    // Eliminamos "public bool Activo { get; set; } = true;" porque ya viene de BaseEntity[cite: 1]

    public ICollection<AsignacionCandidatoPuesto> AsignacionesPuestos { get; set; } = [];
}