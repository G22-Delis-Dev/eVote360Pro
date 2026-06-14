using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.AsignacionCandidatos;
using eVote360Pro.Domain.Exceptions; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Dirigente;

public class AsignacionesController : Controller
{
    private readonly IAsignacionCandidatoPuestoService _asignacionService;
    private readonly ICandidatoService _candidatoService;
    private readonly IPartidoPoliticoService _partidoService;
    private readonly IPuestoElectivoService _puestoService;
    private readonly IMapper _mapper;

    public AsignacionesController(
        IAsignacionCandidatoPuestoService asignacionService,
        ICandidatoService candidatoService,
        IPartidoPoliticoService partidoService,
        IPuestoElectivoService puestoService,
        IMapper mapper)
    {
        _asignacionService = asignacionService;
        _candidatoService = candidatoService;
        _partidoService = partidoService;
        _puestoService = puestoService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var dtos = await _asignacionService.ObtenerTodasAsync();
        var listaVms = _mapper.Map<IEnumerable<AsignacionCandidatoListViewModel>>(dtos);
        return View(listaVms);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarDropdownsAsync();
        return View(new AsignacionCandidatoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AsignacionCandidatoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.PartidoPoliticoId);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AsignacionCandidatoPuestoDto>(vm);
            await _asignacionService.CrearAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.PartidoPoliticoId);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _asignacionService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        var vm = _mapper.Map<AsignacionCandidatoEditViewModel>(dto);
        await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.PartidoPoliticoId);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AsignacionCandidatoEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.PartidoPoliticoId);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AsignacionCandidatoPuestoDto>(vm);
            await _asignacionService.ActualizarAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId, vm.PartidoPoliticoId);
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
        await _asignacionService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }
    private async Task CargarDropdownsAsync(int? candidatoId = null, int? puestoId = null, int? partidoId = null)
    {
        var candidatos = await _candidatoService.ObtenerTodosAsync();
        var partidos = await _partidoService.ObtenerTodosAsync();
        var puestos = await _puestoService.ObtenerTodosAsync();

        ViewBag.Candidatos = new SelectList(candidatos, "Id", "Nombre", candidatoId);
        ViewBag.Partidos = new SelectList(partidos.Where(p => p.Activo), "Id", "Nombre", partidoId);
        ViewBag.Puestos = new SelectList(puestos.Where(p => p.Activo), "Id", "Nombre", puestoId);
    }
}