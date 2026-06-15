using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Home;
using eVote360Pro.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace eVote360Pro.Web.Controllers.Dirigente;

public class HomeDirigenteController : Controller
{
    private readonly IPartidoPoliticoService _partidoService;
    private readonly ICandidatoService _candidatoService;
    private readonly IAlianzaPoliticaService _alianzaService;
    private readonly IAsignacionCandidatoPuestoService _asignacionService;
    private readonly IEleccionService? _eleccionService;

    public HomeDirigenteController(
        IPartidoPoliticoService partidoService,
        ICandidatoService candidatoService,
        IAlianzaPoliticaService alianzaService,
        IAsignacionCandidatoPuestoService asignacionService,
        IEleccionService? eleccionService = null) 
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

        // Construimos el ViewModel recolectando las estadísticas
        var viewModel = new HomeDirigenteViewModel
        {
            TotalPartidosPoliticos = partidos.Count(p => p.Activo),
            TotalCandidatos = candidatos.Count(),
            // Contamos solo las alianzas que están esperando respuesta (Estado = Pendiente)
            TotalAlianzasPendientes = alianzas.Count(a => a.Estado == EstadoAlianza.Pendiente),
            TotalAsignaciones = asignaciones.Count()
        };

        // Si el servicio de elecciones ya está desarrollado, extraemos sus datos activos
        if (_eleccionService != null)
        {
            // TODO: Descomentar esto cuando Delis termine el IEleccionService
            // var elecciones = await _eleccionService.ObtenerTodasAsync();
            // viewModel.TotalEleccionesActivas = elecciones.Count(e => e.Estado == EstadoEleccion.Activa); 

            viewModel.TotalEleccionesActivas = 0; // Valor temporal para que compile
        }

        // Enviamos el panel consolidado directamente a la vista del Dirigente
        return View(viewModel);
    }
}