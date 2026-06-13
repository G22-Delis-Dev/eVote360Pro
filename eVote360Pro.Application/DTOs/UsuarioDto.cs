namespace eVote360Pro.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public bool Activo { get; set; }
}