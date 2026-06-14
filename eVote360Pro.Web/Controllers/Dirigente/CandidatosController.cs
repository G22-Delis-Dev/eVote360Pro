using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Candidatos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace eVote360Pro.Web.Controllers.Dirigente;

public class CandidatosController : Controller
{
    private readonly ICandidatoService _candidatoService;
    private readonly IPartidoPoliticoService _partidoService; // Para el dropdown
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment; // Para guardar las imágenes

    public CandidatosController(
        ICandidatoService candidatoService,
        IPartidoPoliticoService partidoService,
        IMapper mapper,
        IWebHostEnvironment webHostEnvironment)
    {
        _candidatoService = candidatoService;
        _partidoService = partidoService;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var candidatos = await _candidatoService.ObtenerTodosAsync();
        return View(candidatos);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Llenar el Dropdown de Partidos
        await CargarDropdownPartidosAsync();
        return View(new CandidatoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CandidatoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownPartidosAsync();
            return View(vm);
        }

        // Convertimos el ViewModel a DTO
        var dto = _mapper.Map<CandidatoDto>(vm);

        // Lógica de guardado de imagen
        if (vm.FotoArchivo != null)
        {
            dto.FotoUrl = await GuardarFotoAsync(vm.FotoArchivo);
        }

        await _candidatoService.CrearAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var candidatoDto = await _candidatoService.ObtenerPorIdAsync(id);
        if (candidatoDto == null) return NotFound();

        var vm = _mapper.Map<CandidatoCreateViewModel>(candidatoDto);
        vm.FotoUrlExistente = candidatoDto.FotoUrl; // Guardamos la ruta por si no sube foto nueva

        await CargarDropdownPartidosAsync(vm.PartidoPoliticoId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CandidatoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownPartidosAsync(vm.PartidoPoliticoId);
            return View(vm);
        }

        var dto = _mapper.Map<CandidatoDto>(vm);

        // Si se sube una foto nueva, la guardamos. Si no, mantenemos la que ya estaba.
        if (vm.FotoArchivo != null)
        {
            dto.FotoUrl = await GuardarFotoAsync(vm.FotoArchivo);
        }
        else
        {
            dto.FotoUrl = vm.FotoUrlExistente;
        }

        await _candidatoService.ActualizarAsync(id, dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        await _candidatoService.CambiarEstadoAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarDropdownPartidosAsync(int? partidoSeleccionado = null)
    {
        // Solo mostramos partidos activos en el dropdown
        var todosPartidos = await _partidoService.ObtenerTodosAsync();

        var partidosActivos = todosPartidos.Where(p => p.Activo == true).ToList();

        ViewBag.Partidos = new SelectList(partidosActivos, "Id", "Nombre", partidoSeleccionado);
    }

    private async Task<string> GuardarFotoAsync(IFormFile foto)
    {
        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "candidatos");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        // Creamos un nombre único para evitar que dos fotos se llamen igual
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + foto.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await foto.CopyToAsync(fileStream);
        }

        return $"/img/candidatos/{uniqueFileName}";
    }
}