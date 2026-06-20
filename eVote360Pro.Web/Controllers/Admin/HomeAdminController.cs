using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Home;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers.Admin;

[eVote360Pro.Web.Filters.ValidarSesion("Administrador")]
public class HomeAdminController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly ICiudadanoService _ciudadanoService;
    private readonly IPartidoPoliticoService _partidoService;
    private readonly IPuestoElectivoService _puestoService;
    private readonly IAsignacionDirigenteService _asignacionService;
    private readonly IEleccionService _eleccionService;

    public HomeAdminController(
        IUsuarioService usuarioService,
        ICiudadanoService ciudadanoService,
        IPartidoPoliticoService partidoService,
        IPuestoElectivoService puestoService,
        IAsignacionDirigenteService asignacionService,
        IEleccionService eleccionService)
    {
        _usuarioService = usuarioService;
        _ciudadanoService = ciudadanoService;
        _partidoService = partidoService;
        _puestoService = puestoService;
        _asignacionService = asignacionService;
        _eleccionService = eleccionService;
    }

    public async Task<IActionResult> Index(int? anio)
    {
        int anioConsulta = anio ?? DateTime.Now.Year;

        var usuarios     = await _usuarioService.ObtenerListaAsync();
        var ciudadanos   = await _ciudadanoService.ObtenerListaAsync();
        var partidos     = await _partidoService.ObtenerTodosAsync();
        var puestos      = await _puestoService.ObtenerTodosAsync();
        var asignaciones = await _asignacionService.ObtenerListaAsync();
        var elecciones   = await _eleccionService.ObtenerTodosAsync();
        var resumenAnio  = await _eleccionService.ObtenerResumenPorAnioAsync(anioConsulta);

        var viewModel = new HomeAdminViewModel
        {
            TotalUsuarios               = usuarios.Count(),
            TotalCiudadanos             = ciudadanos.Count(),
            TotalPartidosPoliticos      = partidos.Count(p => p.Activo),
            TotalPuestosElectivos       = puestos.Count(p => p.Activo),
            TotalAsignacionesDirigentes = asignaciones.Count(),

            TotalElecciones       = elecciones.Count(),
            EleccionesActivas     = elecciones.Count(e => e.Estado == EstadoEleccion.Activa),
            EleccionesFinalizadas = elecciones.Count(e => e.Estado == EstadoEleccion.Finalizada),
            EleccionesPendientes  = elecciones.Count(e => e.Estado == EstadoEleccion.Pendiente),

            AnioConsulta = anioConsulta,
            ResumenElecciones = resumenAnio.Select(r => new ResumenEleccionItemViewModel
            {
                Id              = r.Id,
                NombreEleccion  = r.NombreEleccion,
                Estado          = r.Estado,
                TotalPartidos   = r.TotalPartidos,
                TotalCandidatos = r.TotalCandidatos,
                TotalVotantes   = r.TotalVotantes
            })
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> TestEmail(string destinatario, [FromServices] IEmailService emailService)
    {
        if (string.IsNullOrEmpty(destinatario))
            return Content("Por favor, proporciona un correo de destino. Ejemplo: /HomeAdmin/TestEmail?destinatario=tu_correo@gmail.com");

        try
        {
            await emailService.EnviarAsync(destinatario, "Prueba de eVote360 Pro",
                "<h3>¡Hola!</h3><p>Esta es una prueba de correo electrónico de <strong>eVote360 Pro</strong>.</p>");
            return Content($"Correo de prueba enviado con éxito a: {destinatario}");
        }
        catch (Exception ex)
        {
            return Content($"Error al enviar correo: {ex.Message}");
        }
    }
}
