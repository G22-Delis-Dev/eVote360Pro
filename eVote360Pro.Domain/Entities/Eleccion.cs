using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Domain.Entities;

public class Eleccion : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public EstadoEleccion Estado { get; set; } = EstadoEleccion.Pendiente;
    public DateTime? FechaActivacion { get; set; }
    public DateTime? FechaFinalizacion { get; set; }

    // Navegación
    public ICollection<EleccionPuesto> EleccionPuestos { get; set; } = [];
    public ICollection<CodigoVerificacion> CodigosVerificacion { get; set; } = [];
    public ICollection<Voto> Votos { get; set; } = [];
    public ICollection<ParticipacionElectoral> ParticipacionesElectorales { get; set; } = [];
}