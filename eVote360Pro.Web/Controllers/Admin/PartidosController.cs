using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Partidos;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using eVote360Pro.Web.Helpers;

namespace eVote360Pro.Web.Controllers.Admin;

[eVote360Pro.Web.Filters.ValidarSesion("Administrador")]
public class PartidosController : Controller
{
    private readonly IPartidoPoliticoService _partidoService;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PartidosController(
        IPartidoPoliticoService partidoService,
        IMapper mapper,
        IWebHostEnvironment webHostEnvironment)
    {
        _partidoService = partidoService;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var dtos = await _partidoService.ObtenerTodosAsync();
        var items = _mapper.Map<IEnumerable<PartidoItemViewModel>>(dtos);

        var vm = new PartidoListViewModel
        {
            Partidos = items
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PartidoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartidoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<PartidoPoliticoDto>(vm);
            string rutaLogo = SubidaArchivo.Subir(vm.LogoArchivo, "partidos")!;

            await _partidoService.CrearAsync(dto, rutaLogo);
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
        var dto = await _partidoService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        var participoEnEleccion = await _partidoService.ParticipoEnEleccionAsync(id);

        var vm = _mapper.Map<PartidoEditViewModel>(dto);
        vm.LogoActualRuta = dto.LogoRuta;
        vm.CamposCriticosEditables = !participoEnEleccion;
        
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PartidoEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<PartidoPoliticoDto>(vm);
            string? rutaLogo = SubidaArchivo.Subir(vm.NuevoLogoArchivo, "partidos", isEditMode: true, imagePath: vm.LogoActualRuta);

            await _partidoService.EditarAsync(dto, rutaLogo);
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
            await _partidoService.CambiarEstadoAsync(id);
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
