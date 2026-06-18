using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.PuestosElectivos;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers.Admin;

public class PuestosElectivosController : Controller
{
    private readonly IPuestoElectivoService _puestoService;
    private readonly IMapper _mapper;

    public PuestosElectivosController(
        IPuestoElectivoService puestoService,
        IMapper mapper)
    {
        _puestoService = puestoService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var dtos = await _puestoService.ObtenerTodosAsync();
        var items = _mapper.Map<IEnumerable<PuestoElectivoItemViewModel>>(dtos);

        var vm = new PuestoElectivoListViewModel
        {
            Puestos = items
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PuestoElectivoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PuestoElectivoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<PuestoElectivoDto>(vm);
            await _puestoService.CrearAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _puestoService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        var vm = _mapper.Map<PuestoElectivoEditViewModel>(dto);
        vm.NombreEsEditable = !await _puestoService.ParticipoEnEleccionAsync(id);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PuestoElectivoEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.NombreEsEditable = !await _puestoService.ParticipoEnEleccionAsync(id);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<PuestoElectivoDto>(vm);
            await _puestoService.ActualizarAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            vm.NombreEsEditable = !await _puestoService.ParticipoEnEleccionAsync(id);
            return View(vm);
        }
        catch (RegistroNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        try
        {
            await _puestoService.CambiarEstadoAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (RegistroNoEncontradoException)
        {
            return NotFound();
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
