using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Home;
using eVote360Pro.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers.Dirigente;

[eVote360Pro.Web.Filters.ValidarSesion("DirigentePolitico")]
public class HomeDirigenteController : Controller
{
    private readonly IPartidoPoliticoService _partidoService;
    private readonly ICandidatoService _candidatoService;
    private readonly IAlianzaPoliticaService _alianzaService;
    private readonly IAsignacionCandidatoPuestoService _asignacionService;
    private readonly IEleccionService _eleccionService;

    public HomeDirigenteController(
        IPartidoPoliticoService partidoService,
        ICandidatoService candidatoService,
        IAlianzaPoliticaService alianzaService,
        IAsignacionCandidatoPuestoService asignacionService,
        IEleccionService eleccionService) 
    {
        _partidoService = partidoService;
        _candidatoService = candidatoService;
        _alianzaService = alianzaService;
        _asignacionService = asignacionService;
        _eleccionService = eleccionService;
    }

    public async Task<IActionResult> Index()
    {
        // Buscamos las listas de datos desde los servicios de aplicación
        var partidos = await _partidoService.ObtenerTodosAsync();
        var candidatos = await _candidatoService.ObtenerTodosAsync();
        var alianzas = await _alianzaService.ObtenerTodosAsync();
        var asignaciones = await _asignacionService.ObtenerTodosAsync();
        var elecciones = await _eleccionService.ObtenerTodosAsync();

        var hayEleccionActiva = elecciones.Any(e => e.Estado == EstadoEleccion.Activa);

        // Construimos el ViewModel recolectando las estadísticas
        var viewModel = new HomeDirigenteViewModel
        {
            TotalPartidosPoliticos = partidos.Count(p => p.Activo),
            TotalCandidatos = candidatos.Count(),
            // Contamos solo las alianzas que están esperando respuesta (Estado = Pendiente)
            TotalAlianzasPendientes = alianzas.Count(a => a.Estado == EstadoAlianza.Pendiente),
            TotalAsignaciones = asignaciones.Count(),
            // Contamos solo las elecciones que están en estado Activa
            TotalEleccionesActivas = elecciones.Count(e => e.Estado == EstadoEleccion.Activa),
            HayEleccionActiva = hayEleccionActiva
        };

        ViewBag.HayEleccionActiva = hayEleccionActiva;

        // Enviamos el panel consolidado directamente a la vista del Dirigente
        return View(viewModel);
    }
}
