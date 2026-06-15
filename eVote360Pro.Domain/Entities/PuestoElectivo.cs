namespace eVote360Pro.Domain.Entities;

public class PuestoElectivo : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // Navegación
    public ICollection<EleccionPuesto> EleccionPuestos { get; set; } = [];
    public ICollection<AsignacionCandidatoPuesto> AsignacionesCandidatos { get; set; } = [];
}