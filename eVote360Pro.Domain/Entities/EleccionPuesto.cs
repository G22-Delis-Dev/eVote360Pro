namespace eVote360Pro.Domain.Entities;

public class EleccionPuesto : BaseEntity
{
    public int EleccionId { get; set; }
    public Eleccion Eleccion { get; set; } = null!;

    public int PuestoElectivoId { get; set; }
    public PuestoElectivo PuestoElectivo { get; set; } = null!;
}