using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Domain.Entities;
public class AlianzaPolitica : BaseEntity
{
    public int PartidoSolicitanteId { get; set; }
    public PartidoPolitico PartidoSolicitante { get; set; } = null!;

    public int PartidoReceptorId { get; set; }
    public PartidoPolitico PartidoReceptor { get; set; } = null!;

    public EstadoAlianza Estado { get; set; } = EstadoAlianza.Pendiente;
    public DateTime? FechaRespuesta { get; set; }
}