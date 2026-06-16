using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Ciudadanos;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers.Admin;

public class CiudadanosController : Controller
{
    private readonly ICiudadanoService _ciudadanoService;
    private readonly IMapper _mapper;

    public CiudadanosController(
        ICiudadanoService ciudadanoService,
        IMapper mapper)
    {
        _ciudadanoService = ciudadanoService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(string? filtro)
    {
        var dtos = await _ciudadanoService.ObtenerListaAsync(filtro);
        var items = _mapper.Map<IEnumerable<CiudadanoItemViewModel>>(dtos);

        var vm = new CiudadanoListViewModel
        {
            Ciudadanos = items,
            Filtro = filtro
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CiudadanoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CiudadanoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<CiudadanoDto>(vm);
            await _ciudadanoService.CrearAsync(dto);
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
        var dto = await _ciudadanoService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        var vm = _mapper.Map<CiudadanoEditViewModel>(dto);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CiudadanoEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<CiudadanoDto>(vm);
            await _ciudadanoService.ActualizarAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
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
            await _ciudadanoService.CambiarEstadoAsync(id);
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
