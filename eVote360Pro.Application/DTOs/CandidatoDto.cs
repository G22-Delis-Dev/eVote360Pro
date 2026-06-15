namespace eVote360Pro.Application.DTOs;

public class CandidatoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string FotoUrl { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int PartidoPoliticoId { get; set; }
    public string NombrePartido { get; set; } = string.Empty;
    public string LogoPartido { get; set; } = string.Empty;
}