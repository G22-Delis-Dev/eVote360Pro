namespace eVote360Pro.Application.ViewModels.Elecciones;

public class EleccionResultadoViewModel
{
    public int EleccionId { get; set; }
    public string NombreEleccion { get; set; } = string.Empty;
    public int TotalVotantes { get; set; }
    public IEnumerable<ResultadoPuestoViewModel> Puestos { get; set; } = [];

    public int TotalVotosEmitidos { get => TotalVotantes; set => TotalVotantes = value; }
    public IEnumerable<ResultadoPuestoViewModel> ResultadosPorPuesto { get => Puestos; set => Puestos = value; }
}

public class ResultadoPuestoViewModel
{
    public string NombrePuesto { get; set; } = string.Empty;
    public int TotalVotos { get; set; }
    public int TotalVotosPuesto { get => TotalVotos; set => TotalVotos = value; }
    public string PuestoNombre { get => NombrePuesto; set => NombrePuesto = value; }
    public IEnumerable<ResultadoCandidatoViewModel> Candidatos { get; set; } = [];
}

public class ResultadoCandidatoViewModel
{
    public string NombreCandidato { get; set; } = string.Empty;
    public string NombrePartido { get; set; } = string.Empty;
    public string LogoPartido { get; set; } = string.Empty;
    public int TotalVotos { get; set; }
    public double Porcentaje { get; set; }
    public bool EsGanador { get; set; }
    public bool EsEmpate { get; set; }

    public string FotoCandidatoUrl { get; set; } = string.Empty;
    public double PorcentajeVotos { get => Porcentaje; set => Porcentaje = value; }
    public int VotosObtenidos { get => TotalVotos; set => TotalVotos = value; }
    public int Votos { get => TotalVotos; set => TotalVotos = value; }
    public string LogoPartidoUrl { get => LogoPartido; set => LogoPartido = value; }
    public bool EsAlianza { get; set; }

    public int CandidatoId { get; set; }
    public string CandidatoNombre { get => NombreCandidato; set => NombreCandidato = value; }
    public string PartidoNombre { get => NombrePartido; set => NombrePartido = value; }
}