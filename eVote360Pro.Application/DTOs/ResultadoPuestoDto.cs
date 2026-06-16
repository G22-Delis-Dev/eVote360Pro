namespace eVote360Pro.Application.DTOs;

public class ResultadoPuestoDto
{
    public int PuestoId { get; set; }
    public string NombrePuesto { get; set; } = string.Empty;
    public int TotalVotos { get; set; }
    public IEnumerable<ResultadoCandidatoDto> Candidatos { get; set; } = new List<ResultadoCandidatoDto>();
}