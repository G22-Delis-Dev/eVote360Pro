namespace eVote360Pro.Application.DTOs;

public class CiudadanoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public bool Activo { get; set; }
}