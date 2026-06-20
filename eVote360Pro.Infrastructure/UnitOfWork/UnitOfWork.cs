using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;
using eVote360Pro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage; // Necesario para las transacciones

namespace eVote360Pro.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction; // Variable para controlar la transacción

    // Campos privados para los repositorios
    private ICiudadanoRepository? _ciudadanos;
    private IUsuarioRepository? _usuarios;
    private IPartidoPoliticoRepository? _partidosPoliticos;
    private IPuestoElectivoRepository? _puestosElectivos;
    private ICandidatoRepository? _candidatos;
    private IEleccionRepository? _elecciones;
    private IEleccionPuestoRepository? _eleccionPuestos;
    private IVotoRepository? _votos;
    private ICodigoVerificacionRepository? _codigosVerificacion;
    private IAsignacionDirigenteRepository? _asignacionesDirigentes;
    private IAsignacionCandidatoPuestoRepository? _asignacionesCandidatos;
    private IAlianzaPoliticaRepository? _alianzasPoliticas;
    private IParticipacionElectoralRepository? _participacionesElectorales;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Inicialización perezosa (Lazy loading)
    public ICiudadanoRepository Ciudadanos => _ciudadanos ??= new CiudadanoRepository(_context);
    public IUsuarioRepository Usuarios => _usuarios ??= new UsuarioRepository(_context);
    public IPartidoPoliticoRepository PartidosPoliticos => _partidosPoliticos ??= new PartidoPoliticoRepository(_context);
    public IPuestoElectivoRepository PuestosElectivos => _puestosElectivos ??= new PuestoElectivoRepository(_context);
    public ICandidatoRepository Candidatos => _candidatos ??= new CandidatoRepository(_context);
    public IEleccionRepository Elecciones => _elecciones ??= new EleccionRepository(_context);
    public IEleccionPuestoRepository EleccionPuestos => _eleccionPuestos ??= new EleccionPuestoRepository(_context);
    public IVotoRepository Votos => _votos ??= new VotoRepository(_context);
    public ICodigoVerificacionRepository CodigosVerificacion => _codigosVerificacion ??= new CodigoVerificacionRepository(_context);
    public IAsignacionDirigenteRepository AsignacionesDirigentes => _asignacionesDirigentes ??= new AsignacionDirigenteRepository(_context);
    public IAsignacionCandidatoPuestoRepository AsignacionesCandidatos => _asignacionesCandidatos ??= new AsignacionCandidatoPuestoRepository(_context);
    public IAlianzaPoliticaRepository AlianzasPoliticas => _alianzasPoliticas ??= new AlianzaPoliticaRepository(_context);
    public IParticipacionElectoralRepository ParticipacionesElectorales => _participacionesElectorales ??= new ParticipacionElectoralRepository(_context);

    // Guarda los cambios normales
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this); // Esto es para optimizar el Garbage Collector
    }
}