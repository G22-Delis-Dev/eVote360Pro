namespace eVote360Pro.Application.DTOs;

public class AsignacionDirigenteDto
{
    public int Id { get; set; }

    // Necesitamos los IDs para las relaciones
    public int UsuarioId { get; set; }
    public int PartidoPoliticoId { get; set; }

    // Propiedades adicionales para mostrar información en las vistas
    public string NombreDirigente { get; set; } = string.Empty;
    public string NombrePartido { get; set; } = string.Empty;
    public string SiglaPartido { get; set; } = string.Empty;
}