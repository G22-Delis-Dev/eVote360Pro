namespace eVote360Pro.Application.DTOs;

public class ResultadoCandidatoDto
{
    public int? CandidatoId { get; set; }
    public string NombreCandidato { get; set; } = string.Empty;
    public string NombrePartido { get; set; } = string.Empty;
    public string LogoPartido { get; set; } = string.Empty;
    public int TotalVotos { get; set; }
    public double Porcentaje { get; set; }
    public bool EsGanador { get; set; }
    public bool EsEmpate { get; set; }
}