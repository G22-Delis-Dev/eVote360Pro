using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Home;
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
}
