namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    ICiudadanoRepository Ciudadanos { get; }
    IUsuarioRepository Usuarios { get; }
    IPartidoPoliticoRepository PartidosPoliticos { get; }
    IPuestoElectivoRepository PuestosElectivos { get; }
    ICandidatoRepository Candidatos { get; }
    IEleccionRepository Elecciones { get; }
    IEleccionPuestoRepository EleccionPuestos { get; }
    IVotoRepository Votos { get; }
    ICodigoVerificacionRepository CodigosVerificacion { get; }
    IAsignacionDirigenteRepository AsignacionesDirigentes { get; }
    IAsignacionCandidatoPuestoRepository AsignacionesCandidatos { get; }
    IAlianzaPoliticaRepository AlianzasPoliticas { get; }
    IParticipacionElectoralRepository ParticipacionesElectorales { get; }

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task<int> SaveChangesAsync();
}