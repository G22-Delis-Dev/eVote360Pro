namespace eVote360Pro.Domain.Entities;

public class Candidato : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? FotoRuta { get; set; }
    public int PartidoPoliticoId { get; set; }
    public PartidoPolitico PartidoPolitico { get; set; } = null!;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<AsignacionCandidatoPuesto> AsignacionesPuestos { get; set; } = [];
}