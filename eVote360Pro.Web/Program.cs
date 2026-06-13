using eVote360Pro.Application.Interfaces;
using eVote360Pro.Infrastructure.Services; // Asegúrate de este namespace
using eVote360Pro.Shared.Services;      // Asegúrate de este namespace

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Email (Settings)
var emailSettings = new EmailSettings
{
    Host = "smtp.tu-servidor.com",
    Puerto = 587,
    CorreoRemitente = "tu-email@dominio.com",
    NombreRemitente = "eVote360 Pro",
    Password = "tu-contraseña"
};

// 2. Registro de Servicios
builder.Services.AddSingleton(emailSettings);

// Registro de EmailService: .NET puede inyectar EmailSettings automáticamente
// si ya está registrado en el contenedor.
builder.Services.AddScoped<IEmailService, EmailService>();

// Registro de OcrService: Como usa Tesseract (pesado), Singleton es vital
builder.Services.AddSingleton<IOcrService>(provider =>
    new OcrService(Path.Combine(builder.Environment.ContentRootPath, "tessdata")));

// 3. MVC y Pipeline
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();