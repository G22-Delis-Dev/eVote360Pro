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

    // TODO: Reemplazar con el ID real del partido del dirigente autenticado
    // cuando se implemente el sistema de autenticación (Claims/Session).
    private int ObtenerPartidoIdDirigente() => 1;

    public async Task<IActionResult> Index()
    {
        int partidoId = ObtenerPartidoIdDirigente();

        // Solo se muestran las alianzas donde el partido del dirigente es solicitante o receptor
        var dtos = await _alianzaService.ObtenerPorPartidoAsync(partidoId);
        var listaVms = _mapper.Map<IEnumerable<AlianzaListViewModel>>(dtos);
        return View(listaVms);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Solo se carga el dropdown de partidos receptores (excluye el partido del dirigente)
        await CargarDropdownPartidosReceptoresAsync();
        return View(new AlianzaCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlianzaCreateViewModel vm)
    {
        int partidoIdDirigente = ObtenerPartidoIdDirigente();

        if (partidoIdDirigente == vm.PartidoReceptorId)
        {
            ModelState.AddModelError(string.Empty, "Un partido político no puede realizar una alianza consigo mismo.");
        }

        if (!ModelState.IsValid)
        {
            await CargarDropdownPartidosReceptoresAsync(vm.PartidoReceptorId);
            return View(vm);
        }

        var dto = _mapper.Map<AlianzaPoliticaDto>(vm);

        // El partido solicitante se toma automáticamente del dirigente autenticado
        dto.PartidoSolicitanteId = partidoIdDirigente;

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

    private async Task CargarDropdownPartidosReceptoresAsync(int? receptorId = null)
    {
        int partidoIdDirigente = ObtenerPartidoIdDirigente();
        var todosPartidos = await _partidoService.ObtenerTodosAsync();

        // Se excluye el partido del dirigente del dropdown de receptores
        var partidosReceptores = todosPartidos
            .Where(p => p.Activo && p.Id != partidoIdDirigente)
            .ToList();

        ViewBag.PartidosReceptores = new SelectList(partidosReceptores, "Id", "Nombre", receptorId);
    }
}