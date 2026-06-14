namespace eVote360Pro.Application.DTOs;

public class ResumenVotacionDto
{
    public string NombreEleccion { get; set; } = string.Empty;
    public DateTime FechaEleccion { get; set; }
    public List<VotoResumenDto> Votos { get; set; } = [];
}