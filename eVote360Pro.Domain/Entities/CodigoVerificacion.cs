namespace eVote360Pro.Domain.Entities;

public class CodigoVerificacion : BaseEntity
{
    public int CiudadanoId { get; set; }
    public Ciudadano Ciudadano { get; set; } = null!;
    public int EleccionId { get; set; }
    public Eleccion Eleccion { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    public DateTime FechaExpiracion { get; set; }
    public bool Utilizado { get; set; } = false;
}