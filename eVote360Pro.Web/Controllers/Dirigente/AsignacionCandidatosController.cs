using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.AsignacionCandidatos;
using eVote360Pro.Domain.Exceptions; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Dirigente;

[eVote360Pro.Web.Filters.ValidarSesion("DirigentePolitico")]
public class AsignacionesController : Controller
{
    private readonly IAsignacionCandidatoPuestoService _asignacionService;
    private readonly ICandidatoService _candidatoService;
    private readonly IPuestoElectivoService _puestoService;
    private readonly IAlianzaPoliticaService _alianzaService;
    private readonly IMapper _mapper;
    private readonly eVote360Pro.Application.Interfaces.ISesionUsuario _sesionUsuario;

    public AsignacionesController(
        IAsignacionCandidatoPuestoService asignacionService,
        ICandidatoService candidatoService,
        IPuestoElectivoService puestoService,
        IAlianzaPoliticaService alianzaService,
        IMapper mapper,
        eVote360Pro.Application.Interfaces.ISesionUsuario sesionUsuario)
    {
        _asignacionService = asignacionService;
        _candidatoService = candidatoService;
        _puestoService = puestoService;
        _alianzaService = alianzaService;
        _mapper = mapper;
        _sesionUsuario = sesionUsuario;
    }

    // TODO: Reemplazar con el ID real del partido del dirigente autenticado
    // cuando se implemente el sistema de autenticación (Claims/Session).
    private int ObtenerPartidoIdDirigente() => _sesionUsuario.ObtenerPartidoId() ?? 0;

    public async Task<IActionResult> Index()
    {
        int partidoId = ObtenerPartidoIdDirigente();

        // Solo se muestran las asignaciones del partido del dirigente
        var dtos = await _asignacionService.ObtenerPorPartidoAsync(partidoId);
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
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AsignacionCandidatoPuestoDto>(vm);

            // El partido se toma automáticamente del dirigente autenticado
            dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

            await _asignacionService.CrearAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _asignacionService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        // Validar que la asignación pertenece al partido del dirigente
        if (dto.PartidoPoliticoId != ObtenerPartidoIdDirigente())
            return Forbid();

        var vm = _mapper.Map<AsignacionCandidatoEditViewModel>(dto);
        await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AsignacionCandidatoEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<AsignacionCandidatoPuestoDto>(vm);

            // El partido se toma automáticamente del dirigente autenticado
            dto.PartidoPoliticoId = ObtenerPartidoIdDirigente();

            await _asignacionService.ActualizarAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm.CandidatoId, vm.PuestoElectivoId);
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

    private async Task CargarDropdownsAsync(int? candidatoId = null, int? puestoId = null)
    {
        int partidoId = ObtenerPartidoIdDirigente();

        // Cargar candidatos del partido del dirigente (propios)
        var candidatosPropios = await _candidatoService.ObtenerPorPartidoAsync(partidoId);
        
        var selectListItems = candidatosPropios.Select(c => new { 
            c.Id, 
            NombreCompleto = c.NombreCompleto 
        }).ToList();
        
        // Cargar candidatos aliados
        var alianzasVigentes = await _alianzaService.ObtenerAlianzasVigentesAsync(partidoId);
        
        foreach (var alianza in alianzasVigentes)
        {
            int partidoAliadoId = alianza.PartidoSolicitanteId == partidoId ? alianza.PartidoReceptorId : alianza.PartidoSolicitanteId;
            var nombreAliado = alianza.PartidoSolicitanteId == partidoId ? alianza.PartidoReceptorNombre : alianza.PartidoSolicitanteNombre;
            
            var candidatosDeAliado = await _candidatoService.ObtenerPorPartidoAsync(partidoAliadoId);
            
            // Añadir prefijo para distinguirlos
            foreach (var c in candidatosDeAliado)
            {
                selectListItems.Add(new { 
                    c.Id, 
                    NombreCompleto = $"[Aliado: {nombreAliado}] {c.NombreCompleto}" 
                });
            }
        }
        
        var puestos = await _puestoService.ObtenerTodosAsync();

        ViewBag.Candidatos = new SelectList(selectListItems, "Id", "NombreCompleto", candidatoId);
        ViewBag.Puestos = new SelectList(puestos.Where(p => p.Activo), "Id", "Nombre", puestoId);
    }
}
