namespace eVote360Pro.Domain.Entities;

public class Ciudadano : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;

    // Navegación
    public ICollection<CodigoVerificacion> CodigosVerificacion { get; set; } = [];
    public ICollection<ParticipacionElectoral> ParticipacionesElectorales { get; set; } = [];
}