namespace eVote360Pro.Application.DTOs;

public class PartidoPoliticoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Siglas { get; set; } = string.Empty;
    public string? LogoRuta { get; set; }
    public bool Activo { get; set; }
}