using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.AsignacionDirigentes;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Admin;

[eVote360Pro.Web.Filters.ValidarSesion("Administrador")]
public class AsignacionDirigentesController : Controller
{
    private readonly IAsignacionDirigenteService _asignacionService;
    private readonly IMapper _mapper;

    public AsignacionDirigentesController(
        IAsignacionDirigenteService asignacionService,
        IMapper mapper)
    {
        _asignacionService = asignacionService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int? partidoFiltroId)
    {
        var dtos = await _asignacionService.ObtenerListaAsync();

        var partidosDisponibles = dtos
            .Select(d => new SelectListItem { Value = d.PartidoPoliticoId.ToString(), Text = d.NombrePartido })
            .DistinctBy(p => p.Value)
            .ToList();

        if (partidoFiltroId.HasValue)
        {
            dtos = dtos.Where(d => d.PartidoPoliticoId == partidoFiltroId.Value);
        }

        var items = _mapper.Map<IEnumerable<AsignacionDirigenteItemViewModel>>(dtos);

        var vm = new AsignacionDirigenteListViewModel
        {
            Asignaciones = items,
            PartidosDisponibles = partidosDisponibles,
            PartidoFiltroId = partidoFiltroId
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new AsignacionDirigenteCreateViewModel();
        await CargarDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AsignacionDirigenteCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync(vm);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AsignacionDirigenteDto>(vm);
            await _asignacionService.CrearAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm);
            return View(vm);
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _asignacionService.EliminarAsync(id);
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

    private async Task CargarDropdownsAsync(AsignacionDirigenteCreateViewModel vm)
    {
        var dirigentes = await _asignacionService.ObtenerDirigentesDisponiblesAsync();
        var partidos = await _asignacionService.ObtenerPartidosDisponiblesAsync();

        vm.DirigentesDisponibles = dirigentes.Select(d => new SelectListItem
        {
            Value = d.Value.ToString(),
            Text = d.Text
        });

        vm.PartidosDisponibles = partidos.Select(p => new SelectListItem
        {
            Value = p.Value.ToString(),
            Text = p.Text
        });
    }
}
