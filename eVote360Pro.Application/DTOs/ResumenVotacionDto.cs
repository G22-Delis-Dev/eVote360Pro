namespace eVote360Pro.Application.DTOs;

public class ResumenVotacionDto
{
    public string NombreEleccion { get; set; } = string.Empty;
    public DateTime FechaEleccion { get; set; }
    public List<VotoResumenDto> Votos { get; set; } = [];
}

public class VotoResumenDto
{
    public string Puesto { get; set; } = string.Empty;
    public string Candidato { get; set; } = string.Empty;
    public string Partido { get; set; } = string.Empty;
}