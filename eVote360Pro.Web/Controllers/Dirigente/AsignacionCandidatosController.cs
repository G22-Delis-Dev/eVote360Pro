using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.AsignacionCandidatos;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Dirigente;

[eVote360Pro.Web.Filters.ValidarSesion("DirigentePolitico")]
public class AsignacionesController : Controller
{
    private readonly IAsignacionCandidatoPuestoService _asignacionService;
    private readonly ICandidatoService _candidatoService;
    private readonly IPuestoElectivoService _puestoService;
    private readonly IEleccionService _eleccionService;
    private readonly IMapper _mapper;
    private readonly ISesionUsuario _sesionUsuario;

    public AsignacionesController(
        IAsignacionCandidatoPuestoService asignacionService,
        ICandidatoService candidatoService,
        IPuestoElectivoService puestoService,
        IEleccionService eleccionService,
        IMapper mapper,
        ISesionUsuario sesionUsuario)
    {
        _asignacionService = asignacionService;
        _candidatoService = candidatoService;
        _puestoService = puestoService;
        _eleccionService = eleccionService;
        _mapper = mapper;
        _sesionUsuario = sesionUsuario;
    }

    private int ObtenerPartidoIdDirigente() => _sesionUsuario.ObtenerPartidoId() ?? 0;

    public async Task<IActionResult> Index()
    {
        int partidoId = ObtenerPartidoIdDirigente();
        var dtos = await _asignacionService.ObtenerPorPartidoAsync(partidoId);
        var listaVms = _mapper.Map<IEnumerable<AsignacionCandidatoListViewModel>>(dtos);
        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();
        return View(listaVms);
    }

    [HttpGet]
    public async Task<IActionResult> Create(bool esAlianza = false)
    {
        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();
        var vm = new AsignacionCandidatoCreateViewModel { EsAliado = esAlianza };
        await CargarDropdownsAsync(esAlianza: esAlianza);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AsignacionCandidatoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.EsAliado);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AsignacionCandidatoPuestoDto>(vm);
            dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

            await _asignacionService.CrearAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.EsAliado);
            return View(vm);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.EsAliado);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _asignacionService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        if (dto.PartidoPoliticoId != ObtenerPartidoIdDirigente())
            return Forbid();

        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();
        var vm = _mapper.Map<AsignacionCandidatoEditViewModel>(dto);
        await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AsignacionCandidatoEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AsignacionCandidatoPuestoDto>(vm);
            dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

            await _asignacionService.ActualizarAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
            return View(vm);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
            return View(vm);
        }
        catch (RegistroNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _asignacionService.EliminarAsync(id);
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarDropdownsAsync(int? candidatoId = null, int? puestoId = null, bool esAlianza = false)
    {
        int partidoId = ObtenerPartidoIdDirigente();
        var puestos = await _puestoService.ObtenerTodosAsync();
        ViewBag.Puestos = new SelectList(puestos.Where(p => p.Activo), "Id", "Nombre", puestoId);

        if (esAlianza)
        {
            var candidatosAliados = await _candidatoService.ObtenerAliadosPorPartidoAsync(partidoId);
            var itemsAliados = candidatosAliados.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"[Aliado: {c.NombrePartido}] {c.NombreCompleto}",
                Selected = c.Id == candidatoId
            }).ToList();

            ViewBag.Candidatos = new SelectList(itemsAliados, "Value", "Text", candidatoId?.ToString());
        }
        else
        {
            var candidatos = await _candidatoService.ObtenerPorPartidoAsync(partidoId);
            ViewBag.Candidatos = new SelectList(candidatos, "Id", "NombreCompleto", candidatoId);
        }
    }
}
