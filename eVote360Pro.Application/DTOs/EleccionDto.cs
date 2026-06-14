using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Application.DTOs;

public class EleccionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public EstadoEleccion Estado { get; set; }
    public DateTime? FechaActivacion { get; set; }
    public DateTime? FechaFinalizacion { get; set; }
}