namespace eVote360Pro.Domain.Entities;

public class PuestoElectivo : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<EleccionPuesto> EleccionPuestos { get; set; } = [];
    public ICollection<AsignacionCandidatoPuesto> AsignacionesCandidatos { get; set; } = [];
}