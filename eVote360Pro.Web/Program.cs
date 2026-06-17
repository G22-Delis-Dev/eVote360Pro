using eVote360Pro.Infrastructure; // Importante para acceder al m�todo de extensi�n

var builder = WebApplication.CreateBuilder(args);

// 1. Inyectar todo desde Infraestructura (incluye DB, Repositorios, Servicios y AutoMapper)
builder.Services.AgregarCapaInfraestructura(builder.Configuration);

// 2. Otros registros que deben ser espec�ficos de la capa Web
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<eVote360Pro.Application.Interfaces.ISesionUsuario, eVote360Pro.Web.Middlewares.SesionUsuario>();

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
