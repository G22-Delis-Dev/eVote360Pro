namespace eVote360Pro.Application.DTOs;

public class ResultadoEleccionDto
{
    public int EleccionId { get; set; }
    public string NombreEleccion { get; set; } = string.Empty;
    public int TotalVotantes { get; set; }
    public IEnumerable<ResultadoPuestoDto> Puestos { get; set; } = new List<ResultadoPuestoDto>();
}