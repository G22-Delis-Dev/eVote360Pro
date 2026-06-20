namespace eVote360Pro.Application.DTOs;

public class ResumenEleccionDto
{
    public int Id { get; set; }
    public string NombreEleccion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int TotalPartidos { get; set; }
    public int TotalCandidatos { get; set; }
    public int TotalVotantes { get; set; }
}