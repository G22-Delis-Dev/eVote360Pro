using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Alianzas;
using eVote360Pro.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Dirigente;

public class AlianzasController : Controller
{
    private readonly IAlianzaPoliticaService _alianzaService;
    private readonly IPartidoPoliticoService _partidoService;
    private readonly IMapper _mapper;

    public AlianzasController(
        IAlianzaPoliticaService alianzaService,
        IPartidoPoliticoService partidoService,
        IMapper mapper)
    {
        _alianzaService = alianzaService;
        _partidoService = partidoService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var dtos = await _alianzaService.ObtenerTodasAsync();
        // Mapeamos los DTOs de la base de datos hacia el ListViewModel que requiere la tabla HTML
        var listaVms = _mapper.Map<IEnumerable<AlianzaListViewModel>>(dtos);
        return View(listaVms);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarDropdownsPartidosAsync();
        return View(new AlianzaCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlianzaCreateViewModel vm)
    {
        if (vm.PartidoSolicitanteId == vm.PartidoReceptorId)
        {
            ModelState.AddModelError(string.Empty, "Un partido político no puede realizar una alianza consigo mismo.");
        }

        if (!ModelState.IsValid)
        {
            await CargarDropdownsPartidosAsync(vm.PartidoSolicitanteId, vm.PartidoReceptorId);
            return View(vm);
        }

        var dto = _mapper.Map<AlianzaPoliticaDto>(vm);
        await _alianzaService.CrearAsync(dto);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Responder(int id, EstadoAlianza nuevoEstado)
    {
        await _alianzaService.ResponderSolicitudAsync(id, nuevoEstado);
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarDropdownsPartidosAsync(int? solicitanteId = null, int? receptorId = null)
    {
        var todosPartidos = await _partidoService.ObtenerTodosAsync();
        var partidosActivos = todosPartidos.Where(p => p.Activo).ToList();

        ViewBag.PartidosSolicitantes = new SelectList(partidosActivos, "Id", "Nombre", solicitanteId);
        ViewBag.PartidosReceptores = new SelectList(partidosActivos, "Id", "Nombre", receptorId);
    }
}