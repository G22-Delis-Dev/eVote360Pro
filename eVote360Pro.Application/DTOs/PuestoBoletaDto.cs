namespace eVote360Pro.Application.DTOs;

public class PuestoBoletaDto
{
    public int PuestoId { get; set; }
    public string PuestoNombre { get; set; } = string.Empty;

    // Lista de candidatos postulados a este puesto específico
    public IEnumerable<CandidatoBoletaDto> Candidatos { get; set; } = new List<CandidatoBoletaDto>();
}