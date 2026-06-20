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
    private readonly ISesionUsuario _sesionUsuario;

    public HomeDirigenteController(
        IPartidoPoliticoService partidoService,
        ICandidatoService candidatoService,
        IAlianzaPoliticaService alianzaService,
        IAsignacionCandidatoPuestoService asignacionService,
        IEleccionService eleccionService,
        ISesionUsuario sesionUsuario) 
    {
        _partidoService = partidoService;
        _candidatoService = candidatoService;
        _alianzaService = alianzaService;
        _asignacionService = asignacionService;
        _eleccionService = eleccionService;
        _sesionUsuario = sesionUsuario;
    }

    public async Task<IActionResult> Index()
    {
        var partidoId = _sesionUsuario.ObtenerPartidoId();

        // Si por alguna razon no tiene partido asignado, los contadores se quedaran en 0
        int totalCandidatos = 0;
        int totalAlianzasPendientes = 0;
        int totalAsignaciones = 0;

        if (partidoId.HasValue)
        {
            var candidatos = await _candidatoService.ObtenerTodosAsync();
            totalCandidatos = candidatos.Count(c => c.PartidoPoliticoId == partidoId.Value);

            var alianzas = await _alianzaService.ObtenerTodosAsync();
            totalAlianzasPendientes = alianzas.Count(a => a.Estado == EstadoAlianza.Pendiente && 
                                                        (a.PartidoReceptorId == partidoId.Value || a.PartidoSolicitanteId == partidoId.Value));

            var asignaciones = await _asignacionService.ObtenerTodosAsync();
            totalAsignaciones = asignaciones.Count(a => a.PartidoPoliticoId == partidoId.Value);
        }

        var partidos = await _partidoService.ObtenerTodosAsync();
        var elecciones = await _eleccionService.ObtenerTodosAsync();

        var hayEleccionActiva = elecciones.Any(e => e.Estado == EstadoEleccion.Activa);

        // Construimos el ViewModel recolectando las estadísticas
        var viewModel = new HomeDirigenteViewModel
        {
            TotalPartidosPoliticos = partidos.Count(p => p.Activo),
            TotalCandidatos = totalCandidatos,
            TotalAlianzasPendientes = totalAlianzasPendientes,
            TotalAsignaciones = totalAsignaciones,
            // Contamos solo las elecciones que están en estado Activa
            TotalEleccionesActivas = elecciones.Count(e => e.Estado == EstadoEleccion.Activa),
            HayEleccionActiva = hayEleccionActiva
        };

        ViewBag.HayEleccionActiva = hayEleccionActiva;

        // Enviamos el panel consolidado directamente a la vista del Dirigente
        return View(viewModel);
    }
}
