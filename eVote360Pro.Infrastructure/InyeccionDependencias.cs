using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.Mappings;
using eVote360Pro.Application.Services;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Data;
using eVote360Pro.Infrastructure.Repositories;
using eVote360Pro.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360Pro.Infrastructure;

public static class InyeccionDependencias
{
    public static IServiceCollection AgregarCapaInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar Base de Datos
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Registrar el Repositorio Genérico y los Específicos
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        // Repositorios
        services.AddScoped<ICiudadanoRepository, CiudadanoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPartidoPoliticoRepository, PartidoPoliticoRepository>();
        services.AddScoped<IPuestoElectivoRepository, PuestoElectivoRepository>();
        services.AddScoped<ICandidatoRepository, CandidatoRepository>();
        services.AddScoped<IEleccionRepository, EleccionRepository>();
        services.AddScoped<IEleccionPuestoRepository, EleccionPuestoRepository>();
        services.AddScoped<IAsignacionDirigenteRepository, AsignacionDirigenteRepository>();
        services.AddScoped<IAlianzaPoliticaRepository, AlianzaPoliticaRepository>();
        services.AddScoped<IAsignacionCandidatoPuestoRepository, AsignacionCandidatoPuestoRepository>();
        services.AddScoped<IVotoRepository, VotoRepository>();
        services.AddScoped<IParticipacionElectoralRepository, ParticipacionElectoralRepository>();
        services.AddScoped<ICodigoVerificacionRepository, CodigoVerificacionRepository>();
        services.AddScoped<IUnitOfWork, eVote360Pro.Infrastructure.UnitOfWork.UnitOfWork>();

        // Servicios
        services.AddScoped<ICandidatoService, CandidatoService>();
        services.AddScoped<IAlianzaPoliticaService, AlianzaPoliticaService>(); // <-- Añadir esta línea


        // Registrar Configuración y Servicio de Email (MailKit)
        var emailSettings = new EmailSettings();
        configuration.GetSection("EmailSettings").Bind(emailSettings);
        services.AddSingleton(emailSettings);
        services.AddTransient<IEmailService, EmailService>();

        // Registrar Mapeo Automático (AutoMapper)
        services.AddAutoMapper(config =>
        {
            config.AddProfile<PerfilGeneral>();
        });
        return services;
    }
}