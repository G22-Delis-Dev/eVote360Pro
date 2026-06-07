namespace eVote360Pro.Domain.Entities;

public class ParticipacionElectoral : BaseEntity
{
    public int CiudadanoId { get; set; }
    public Ciudadano Ciudadano { get; set; } = null!;

    public int EleccionId { get; set; }
    public Eleccion Eleccion { get; set; } = null!;

    public DateTime FechaVoto { get; set; } = DateTime.UtcNow;
}