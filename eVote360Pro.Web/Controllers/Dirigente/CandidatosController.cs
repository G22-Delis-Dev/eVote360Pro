using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Candidatos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Web.Controllers.Dirigente;

public class CandidatosController : Controller
{
    private readonly ICandidatoService _candidatoService;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CandidatosController(
        ICandidatoService candidatoService,
        IMapper mapper,
        IWebHostEnvironment webHostEnvironment)
    {
        _candidatoService = candidatoService;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    // TODO: Reemplazar con el ID real del partido del dirigente autenticado
    // cuando se implemente el sistema de autenticación (Claims/Session).
    private int ObtenerPartidoIdDirigente() => 1;

    public async Task<IActionResult> Index()
    {
        int partidoId = ObtenerPartidoIdDirigente();

        // Solo se muestran los candidatos del partido del dirigente autenticado
        var dtos = await _candidatoService.ObtenerPorPartidoAsync(partidoId);
        var listaVms = _mapper.Map<IEnumerable<CandidatoListViewModel>>(dtos);
        return View(listaVms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        // Ya no se carga dropdown de partidos; el partido se asigna automáticamente
        return View(new CandidatoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CandidatoCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var dto = _mapper.Map<CandidatoDto>(vm);

        // El partido se toma automáticamente del dirigente autenticado
        dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

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

        // Validar que el candidato pertenece al partido del dirigente
        if (candidatoDto.PartidoPoliticoId != ObtenerPartidoIdDirigente())
            return Forbid();

        // Usamos el ViewModel específico para la edición
        var vm = _mapper.Map<CandidatoEditViewModel>(candidatoDto);
        vm.FotoUrlExistente = candidatoDto.FotoUrl;

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CandidatoEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var dto = _mapper.Map<CandidatoDto>(vm);

        // El partido se toma automáticamente del dirigente autenticado
        dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        // Validar que el candidato pertenece al partido del dirigente antes de cambiar estado
        var candidatoDto = await _candidatoService.ObtenerPorIdAsync(id);
        if (candidatoDto == null) return NotFound();

        if (candidatoDto.PartidoPoliticoId != ObtenerPartidoIdDirigente())
            return Forbid();

        await _candidatoService.CambiarEstadoAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GuardarFotoAsync(IFormFile foto)
    {
        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "candidatos");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = Guid.NewGuid().ToString() + "_" + foto.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await foto.CopyToAsync(fileStream);
        }

        return $"/img/candidatos/{uniqueFileName}";
    }
}