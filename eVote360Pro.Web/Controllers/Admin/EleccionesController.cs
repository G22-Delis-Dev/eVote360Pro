using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Elecciones;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Admin;

public class EleccionesController : Controller
{
    private readonly IEleccionService _eleccionService;
    private readonly IPuestoElectivoService _puestoService;
    private readonly IVotacionService _votacionService;
    private readonly IMapper _mapper;

    public EleccionesController(
        IEleccionService eleccionService,
        IPuestoElectivoService puestoService,
        IVotacionService votacionService,
        IMapper mapper)
    {
        _eleccionService = eleccionService;
        _puestoService = puestoService;
        _votacionService = votacionService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        // TODO: Implementar cuando IEleccionService tenga los métodos del CRUD
        // var dtos = await _eleccionService.ObtenerTodasAsync();
        // var items = _mapper.Map<IEnumerable<EleccionItemViewModel>>(dtos);
        // var vm = new EleccionListViewModel
        // {
        //     Elecciones = items,
        //     HayEleccionActiva = items.Any(e => e.Estado == EstadoEleccion.Activa)
        // };
        // return View(vm);

        var vm = new EleccionListViewModel();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new EleccionCreateViewModel();
        await CargarDropdownPuestosAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EleccionCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownPuestosAsync(vm);
            return View(vm);
        }

        try
        {
            // TODO: Implementar cuando IEleccionService tenga el método CrearAsync
            // var dto = _mapper.Map<EleccionDto>(vm);
            // await _eleccionService.CrearAsync(dto, vm.PuestosSeleccionados);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownPuestosAsync(vm);
            return View(vm);
        }
    }

    // TODO: Implementar acciones ActivarEleccion, FinalizarEleccion y Resultados
    // cuando IEleccionService tenga los métodos correspondientes.

    private async Task CargarDropdownPuestosAsync(EleccionCreateViewModel vm)
    {
        var puestos = await _puestoService.ObtenerActivosAsync();
        vm.PuestosDisponibles = puestos.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Nombre,
            Selected = vm.PuestosSeleccionados.Contains(p.Id)
        });
    }
}
