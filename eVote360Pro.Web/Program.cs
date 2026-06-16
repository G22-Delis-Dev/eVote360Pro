using eVote360Pro.Infrastructure; // Importante para acceder al método de extensión

var builder = WebApplication.CreateBuilder(args);

// 1. Inyectar todo desde Infraestructura (incluye DB, Repositorios, Servicios y AutoMapper)
builder.Services.AgregarCapaInfraestructura(builder.Configuration);

// 2. Otros registros que deben ser específicos de la capa Web
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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