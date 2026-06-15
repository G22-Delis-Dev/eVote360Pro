using eVote360Pro.Infrastructure;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Settings;         
using eVote360Pro.Shared.Services;
using eVote360Pro.Infrastructure.Services; 

var builder = WebApplication.CreateBuilder(args);


var emailSettings = new EmailSettings
{
    Host = "smtp.tu-servidor.com",
    Puerto = 587,
    CorreoRemitente = "tu-email@dominio.com",
    NombreRemitente = "eVote360 Pro",
    Password = "tu-contraseña"
};


builder.Services.AddSingleton(emailSettings);
builder.Services.AddScoped<IEmailService, EmailService>();

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