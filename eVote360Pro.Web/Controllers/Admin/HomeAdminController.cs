using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Home;
using eVote360Pro.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers.Admin;

public class HomeAdminController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly ICiudadanoService _ciudadanoService;
    private readonly IPartidoPoliticoService _partidoService;
    private readonly IPuestoElectivoService _puestoService;
    private readonly IAsignacionDirigenteService _asignacionService;

    public HomeAdminController(
        IUsuarioService usuarioService,
        ICiudadanoService ciudadanoService,
        IPartidoPoliticoService partidoService,
        IPuestoElectivoService puestoService,
        IAsignacionDirigenteService asignacionService)
    {
        _usuarioService = usuarioService;
        _ciudadanoService = ciudadanoService;
        _partidoService = partidoService;
        _puestoService = puestoService;
        _asignacionService = asignacionService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.ObtenerListaAsync();
        var ciudadanos = await _ciudadanoService.ObtenerListaAsync();
        var partidos = await _partidoService.ObtenerTodosAsync();
        var puestos = await _puestoService.ObtenerTodosAsync();
        var asignaciones = await _asignacionService.ObtenerListaAsync();

        var viewModel = new HomeAdminViewModel
        {
            TotalUsuarios = usuarios.Count(),
            TotalCiudadanos = ciudadanos.Count(),
            TotalPartidosPoliticos = partidos.Count(p => p.Activo),
            TotalPuestosElectivos = puestos.Count(p => p.Activo),
            TotalAsignacionesDirigentes = asignaciones.Count()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> TestEmail(string destinatario, [FromServices] IEmailService emailService)
    {
        if (string.IsNullOrEmpty(destinatario))
        {
            return Content("Por favor, proporciona un correo de destino en la URL. Ejemplo: /HomeAdmin/TestEmail?destinatario=tu_correo@gmail.com");
        }

        try
        {
            await emailService.EnviarAsync(destinatario, "Prueba de eVote360 Pro", "<h3>¡Hola!</h3><p>Esta es una prueba de correo electrónico para verificar la configuración SMTP del sistema <strong>eVote360 Pro</strong>.</p>");
            return Content($"Correo de prueba enviado con éxito a: {destinatario}");
        }
        catch (Exception ex)
        {
            return Content($"Error al enviar correo: {ex.Message}\n\nDetalles:\n{ex.ToString()}");
        }
    }
}
