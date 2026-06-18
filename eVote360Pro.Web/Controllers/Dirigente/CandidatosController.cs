using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Candidatos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using eVote360Pro.Web.Helpers;
using eVote360Pro.Domain.Exceptions;

namespace eVote360Pro.Web.Controllers.Dirigente;

[eVote360Pro.Web.Filters.ValidarSesion("DirigentePolitico")]
public class CandidatosController : Controller
{
    private readonly ICandidatoService _candidatoService;
    private readonly IMapper _mapper;
    private readonly eVote360Pro.Application.Interfaces.ISesionUsuario _sesionUsuario;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CandidatosController(
        ICandidatoService candidatoService,
        IMapper mapper,
        IWebHostEnvironment webHostEnvironment,
        eVote360Pro.Application.Interfaces.ISesionUsuario sesionUsuario)
    {
        _candidatoService = candidatoService;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _sesionUsuario = sesionUsuario;
    }

    // TODO: Reemplazar con el ID real del partido del dirigente autenticado
    // cuando se implemente el sistema de autenticación (Claims/Session).
    private int ObtenerPartidoIdDirigente() => _sesionUsuario.ObtenerPartidoId() ?? 0;

    public async Task<IActionResult> Index(string filtro = "")
    {
        int partidoId = ObtenerPartidoIdDirigente();

        // Solo se muestran los candidatos del partido del dirigente autenticado
        var dtos = await _candidatoService.ObtenerPorPartidoAsync(partidoId);
        var listaVms = _mapper.Map<IEnumerable<CandidatoListViewModel>>(dtos);

        if (!string.IsNullOrEmpty(filtro))
        {
            listaVms = listaVms.Where(c => c.NombreCompleto.Contains(filtro, StringComparison.OrdinalIgnoreCase));
        }

        var viewModel = new CandidatoListViewModel
        {
            Candidatos = listaVms,
            Filtro = filtro
        };

        return View(viewModel);
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

        try
        {
            var dto = _mapper.Map<CandidatoDto>(vm);

            // El partido se toma automáticamente del dirigente autenticado
            dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

            if (vm.FotoArchivo != null)
            {
                dto.FotoUrl = SubidaArchivo.Subir(vm.FotoArchivo, "candidatos") ?? string.Empty;
            }

            await _candidatoService.CrearAsync(dto);
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

        try
        {
            var dto = _mapper.Map<CandidatoDto>(vm);

            // El partido se toma automáticamente del dirigente autenticado
            dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

            if (vm.FotoArchivo != null)
            {
                dto.FotoUrl = SubidaArchivo.Subir(vm.FotoArchivo, "candidatos", isEditMode: true, imagePath: vm.FotoUrlExistente) ?? string.Empty;
            }
            else
            {
                dto.FotoUrl = vm.FotoUrlExistente ?? string.Empty;
            }

            await _candidatoService.ActualizarAsync(id, dto);
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
        // Validar que el candidato pertenece al partido del dirigente antes de cambiar estado
        var candidatoDto = await _candidatoService.ObtenerPorIdAsync(id);
        if (candidatoDto == null) return NotFound();

        if (candidatoDto.PartidoPoliticoId != ObtenerPartidoIdDirigente())
            return Forbid();

        try
        {
            await _candidatoService.CambiarEstadoAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }
        
        return RedirectToAction(nameof(Index));
    }

}
