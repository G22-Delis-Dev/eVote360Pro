namespace eVote360Pro.Domain.Entities;

public class AsignacionDirigente : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public int PartidoPoliticoId { get; set; }
    public PartidoPolitico PartidoPolitico { get; set; } = null!;
}