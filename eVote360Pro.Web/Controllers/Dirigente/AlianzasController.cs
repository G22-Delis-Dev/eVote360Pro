using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Alianzas;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Dirigente;

[eVote360Pro.Web.Filters.ValidarSesion("DirigentePolitico")]
public class AlianzasController : Controller
{
    private readonly IAlianzaPoliticaService _alianzaService;
    private readonly IPartidoPoliticoService _partidoService;
    private readonly IEleccionService _eleccionService;
    private readonly IMapper _mapper;
    private readonly ISesionUsuario _sesionUsuario;

    public AlianzasController(
        IAlianzaPoliticaService alianzaService,
        IPartidoPoliticoService partidoService,
        IEleccionService eleccionService,
        IMapper mapper,
        ISesionUsuario sesionUsuario)
    {
        _alianzaService = alianzaService;
        _partidoService = partidoService;
        _eleccionService = eleccionService;
        _mapper = mapper;
        _sesionUsuario = sesionUsuario;
    }

    private int ObtenerPartidoIdDirigente() => _sesionUsuario.ObtenerPartidoId() ?? 0;

    public async Task<IActionResult> Index()
    {
        int partidoId = ObtenerPartidoIdDirigente();

        var dtos = await _alianzaService.ObtenerPorPartidoAsync(partidoId);
        var listaVms = _mapper.Map<IEnumerable<AlianzaListViewModel>>(dtos);
        ViewBag.PartidoActualId = partidoId;
        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();
        return View(listaVms);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();
        await CargarDropdownPartidosReceptoresAsync();
        return View(new AlianzaCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlianzaCreateViewModel vm)
    {
        int partidoIdDirigente = ObtenerPartidoIdDirigente();

        if (partidoIdDirigente == vm.PartidoReceptorId)
            ModelState.AddModelError(string.Empty, "Un partido político no puede realizar una alianza consigo mismo.");

        if (!ModelState.IsValid)
        {
            await CargarDropdownPartidosReceptoresAsync(vm.PartidoReceptorId);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AlianzaPoliticaDto>(vm);
            dto.PartidoSolicitanteId = partidoIdDirigente;

            await _alianzaService.CrearAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownPartidosReceptoresAsync(vm.PartidoReceptorId);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Responder(int id, EstadoAlianza nuevoEstado)
    {
        try
        {
            await _alianzaService.ResponderSolicitudAsync(id, nuevoEstado);
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            await _alianzaService.CancelarSolicitudAsync(id, ObtenerPartidoIdDirigente());
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Romper(int id)
    {
        try
        {
            await _alianzaService.RomperAlianzaAsync(id, ObtenerPartidoIdDirigente());
            TempData["Success"] = "La alianza ha sido rota exitosamente.";
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarDropdownPartidosReceptoresAsync(int? receptorId = null)
    {
        int partidoIdDirigente = ObtenerPartidoIdDirigente();
        var todosPartidos = await _partidoService.ObtenerTodosAsync();

        var partidosReceptores = todosPartidos
            .Where(p => p.Activo && p.Id != partidoIdDirigente)
            .ToList();

        ViewBag.PartidosReceptores = new SelectList(partidosReceptores, "Id", "Nombre", receptorId);
    }
}
