namespace eVote360Pro.Application.ViewModels.Votacion;

public class BoletaElectoralViewModel
{
    public int CiudadanoId { get; set; }
    public int VotanteId { get; set; }
    public int EleccionId { get; set; }
    public string TokenAutenticacion { get; set; } = string.Empty;

    public List<PuestoBoletaViewModel> Puestos { get; set; } = new();

    // Almacena qué candidato/partido eligió para cada puesto electivo
    public List<SeleccionVotoViewModel> Selecciones { get; set; } = new();
}

public class PuestoBoletaViewModel
{
    public int PuestoId { get; set; }
    public string PuestoNombre { get; set; } = string.Empty;
    public int? CandidatoSeleccionadoId { get; set; }
    public List<CandidatoBoletaViewModel> Candidatos { get; set; } = new();
}

public class CandidatoBoletaViewModel
{
    public int CandidatoId { get; set; }
    public string CandidatoNombre { get; set; } = string.Empty;
    public string FotoCandidatoUrl { get; set; } = string.Empty;
    public int PartidoPoliticoId { get; set; }
    public string PartidoNombre { get; set; } = string.Empty;
    public string LogoPartidoUrl { get; set; } = string.Empty;
}