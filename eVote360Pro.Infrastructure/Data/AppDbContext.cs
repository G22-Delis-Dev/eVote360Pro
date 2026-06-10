using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ciudadano> Ciudadanos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<PartidoPolitico> PartidosPoliticos { get; set; }
    public DbSet<PuestoElectivo> PuestosElectivos { get; set; }
    public DbSet<Candidato> Candidatos { get; set; }
    public DbSet<Eleccion> Elecciones { get; set; }
    public DbSet<EleccionPuesto> EleccionPuestos { get; set; }
    public DbSet<AsignacionDirigente> AsignacionesDirigentes { get; set; }
    public DbSet<AlianzaPolitica> AlianzasPoliticas { get; set; }
    public DbSet<AsignacionCandidatoPuesto> AsignacionesCandidatosPuestos { get; set; }
    public DbSet<CodigoVerificacion> CodigosVerificacion { get; set; }
    public DbSet<Voto> Votos { get; set; }
    public DbSet<ParticipacionElectoral> ParticipacionesElectorales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Registra automáticamente todas las configuraciones del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}