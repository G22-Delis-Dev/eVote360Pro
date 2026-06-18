using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Application.ViewModels.Alianzas;

public class AlianzaListViewModel
{
    public int Id { get; set; }
    public int PartidoSolicitanteId { get; set; }
    public string PartidoSolicitanteNombre { get; set; } = string.Empty;
    public int PartidoReceptorId { get; set; }
    public string PartidoReceptorNombre { get; set; } = string.Empty;
    public EstadoAlianza Estado { get; set; }
    public DateTime? FechaRespuesta { get; set; }
}