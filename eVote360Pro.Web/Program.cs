using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.Services;
using eVote360Pro.Domain.Settings;
using eVote360Pro.Infrastructure.Services;
using eVote360Pro.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos (Asegúrate de tener el ConnectionString en appsettings.json)
builder.Services.AddDbContext<eVote360Pro.Infrastructure.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configuración de Settings (Inyección de opciones)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 3. AutoMapper (Escanea los perfiles en el ensamblado de Application)
// Registrar AutoMapper escaneando el ensamblado donde está PerfilGeneral
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<eVote360Pro.Application.Mappings.PerfilGeneral>());

// 4. Servicios Propios (Lógica de Negocio e Infraestructura)
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>(); // Si lo tienes implementado
builder.Services.AddScoped<IVotacionService, VotacionService>();
// Agrega aquí cualquier otro servicio que hayas creado en Application/Infrastructure

// 5. OCR Service (Configuración específica)
builder.Services.AddSingleton<IOcrService>(provider =>
    new OcrService(Path.Combine(builder.Environment.ContentRootPath, "tessdata")));

// 6. MVC y Vistas
builder.Services.AddControllersWithViews();

// 7. Seguridad y Sesiones (Indispensable para un sistema de votación)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 8. Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Si usas Identity
app.UseAuthorization();
app.UseSession(); // Habilitar sesiones para el flujo de votación

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();