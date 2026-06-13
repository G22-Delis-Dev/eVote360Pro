using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Infrastructure.Repositories;
using eVote360Pro.Infrastructure.Data;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Shared.Services;
using eVote360Pro.Application.Mappings;

namespace eVote360Pro.Infrastructure;

public static class InyeccionDependencias
{
    public static IServiceCollection AgregarCapaInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configurar Base de Datos
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 2. Registrar el Repositorio Genérico y los Específicos
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

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

        // 3. Registrar Configuración y Servicio de Email (MailKit)
        var emailSettings = new EmailSettings();
        configuration.GetSection("EmailSettings").Bind(emailSettings);
        services.AddSingleton(emailSettings);
        services.AddTransient<IEmailService, EmailService>();

        // 4. Registrar Mapeo Automático (AutoMapper)
        services.AddAutoMapper(config =>
        {
            config.AddProfile<PerfilGeneral>();
        });
        return services;
    }
}