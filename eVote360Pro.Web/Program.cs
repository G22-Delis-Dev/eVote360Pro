using eVote360Pro.Application.Interfaces;
using eVote360Pro.Infrastructure; // Importante para acceder al método de extensión
using eVote360Pro.Infrastructure.Services;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Escuchar en todas las interfaces para acceso desde red local
builder.WebHost.UseUrls("http://0.0.0.0:5071", "https://localhost:7089");

// 1. Inyectar todo desde Infraestructura (incluye DB, Repositorios, Servicios y AutoMapper)
builder.Services.AgregarCapaInfraestructura(builder.Configuration);

// 2. Otros registros específicos de la capa Web
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<eVote360Pro.Application.Interfaces.ISesionUsuario, eVote360Pro.Web.Middlewares.SesionUsuario>();

// Sobreescribir el OcrService para que use la ruta absoluta de tessdata
var tessDataPath = Path.Combine(builder.Environment.ContentRootPath, "tessdata");
builder.Services.AddScoped<IOcrService>(_ => new OcrService(tessDataPath));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<eVote360Pro.Infrastructure.Data.AppDbContext>();
    // Crear administrador por defecto si no existe
    if (!context.Usuarios.Any(u => u.NombreUsuario == "admin"))
    {
        context.Usuarios.Add(new eVote360Pro.Domain.Entities.Usuario
        {
            Nombre = "Super",
            Apellido = "Administrador",
            CorreoElectronico = "admin@evote360.com",
            NombreUsuario = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
            Rol = eVote360Pro.Domain.Enums.RolUsuario.Administrador,
            Activo = true
        });
        context.SaveChanges();
    }
}

// 3. Pipeline HTTP

// Soporte para ngrok y proxies inversos (ForwardedHeaders)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
    // Aceptar cualquier proxy (ngrok, red local, etc.)
    KnownNetworks = { new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("0.0.0.0"), 0) },
    KnownProxies = { }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
