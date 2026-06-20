using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Elecciones;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Admin;

[eVote360Pro.Web.Filters.ValidarSesion("Administrador")]
public class EleccionesController : Controller
{
    private readonly IEleccionService _eleccionService;
    private readonly IPuestoElectivoService _puestoService;
    private readonly IMapper _mapper;

    public EleccionesController(
        IEleccionService eleccionService,
        IPuestoElectivoService puestoService,
        IMapper mapper)
    {
        _eleccionService = eleccionService;
        _puestoService = puestoService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(string? anio, string? estadoFiltro)
    {
        var dtos = await _eleccionService.ObtenerTodosAsync();
        var items = _mapper.Map<IEnumerable<EleccionItemViewModel>>(dtos);

        if (!string.IsNullOrEmpty(anio) && int.TryParse(anio, out int anioInt))
        {
            items = items.Where(e => e.FechaRealizacion.Year == anioInt);
        }

        if (!string.IsNullOrEmpty(estadoFiltro) && Enum.TryParse<EstadoEleccion>(estadoFiltro, out var estadoEnum))
        {
            items = items.Where(e => e.Estado == estadoEnum);
        }

        var hayActiva = dtos.Any(e => e.Estado == EstadoEleccion.Activa);

        var vm = new EleccionListViewModel
        {
            Elecciones = items,
            HayEleccionActiva = hayActiva,
            AnioFiltro = anio,
            EstadoFiltro = estadoFiltro
        };

        ViewBag.HayEleccionActiva = hayActiva;

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
            var dto = _mapper.Map<EleccionDto>(vm);
            await _eleccionService.CrearConPuestosAsync(dto, vm.PuestosSeleccionados);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownPuestosAsync(vm);
            return View(vm);
        }
    }

    public async Task<IActionResult> Iniciar(int id)
    {
        try
        {
            await _eleccionService.ActivarAsync(id);
            TempData["Success"] = "La elección se ha iniciado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Ocurrió un error inesperado al iniciar la elección: " + ex.Message;
            return RedirectToAction(nameof(Index)); 
        }
    }

    public async Task<IActionResult> Finalizar(int id)
    {
        try
        {
            await _eleccionService.FinalizarAsync(id);
            TempData["Success"] = "La elección se ha finalizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Ocurrió un error inesperado al finalizar la elección: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Resultados(int id)
    {
        var dto = await _eleccionService.ObtenerResultadosAsync(id);

        // Mapeamos el DTO de resultados al ViewModel de Resultados
        var vm = _mapper.Map<EleccionResultadoViewModel>(dto);

        return View(vm);
    }

    private async Task CargarDropdownPuestosAsync(EleccionCreateViewModel vm)
    {
        var puestos = await _puestoService.ObtenerTodosAsync(); 
        vm.PuestosDisponibles = puestos.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Nombre,
            Selected = vm.PuestosSeleccionados.Contains(p.Id)
        });
    }
}
