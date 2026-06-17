using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Usuarios;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360Pro.Web.Controllers.Admin;

[eVote360Pro.Web.Filters.ValidarSesion("Administrador")]
public class UsuariosController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly IMapper _mapper;
    private readonly eVote360Pro.Application.Interfaces.ISesionUsuario _sesionUsuario;

    public UsuariosController(
        IUsuarioService usuarioService,
        IMapper mapper,
        eVote360Pro.Application.Interfaces.ISesionUsuario sesionUsuario)
    {
        _usuarioService = usuarioService;
        _mapper = mapper;
        _sesionUsuario = sesionUsuario;
    }

    // TODO: Reemplazar con el ID real del administrador autenticado
    // cuando se implemente el sistema de autenticación (Claims/Session).
    private int ObtenerUsuarioIdActual() => _sesionUsuario.ObtenerUsuarioSesion()?.Id ?? 0;

    public async Task<IActionResult> Index()
    {
        var dtos = await _usuarioService.ObtenerListaAsync();
        var items = _mapper.Map<IEnumerable<UsuarioItemViewModel>>(dtos);

        var vm = new UsuarioListViewModel
        {
            Usuarios = items
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new UsuarioCreateViewModel();
        CargarDropdownRoles(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            CargarDropdownRoles(vm);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<UsuarioDto>(vm);
            await _usuarioService.CrearAsync(dto, vm.Password);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CargarDropdownRoles(vm);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _usuarioService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        var vm = _mapper.Map<UsuarioEditViewModel>(dto);
        vm.Rol = (int)dto.Rol;
        CargarDropdownRoles(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            CargarDropdownRoles(vm);
            return View(vm);
        }

        try
        {
            var dto = _mapper.Map<UsuarioDto>(vm);
            await _usuarioService.EditarAsync(dto, vm.NuevaPassword);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CargarDropdownRoles(vm);
            return View(vm);
        }
        catch (RegistroNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        try
        {
            int usuarioActualId = ObtenerUsuarioIdActual();
            await _usuarioService.ToggleActivoAsync(id, usuarioActualId);
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

    private void CargarDropdownRoles(UsuarioCreateViewModel vm)
    {
        vm.RolesDisponibles = Enum.GetValues<RolUsuario>()
            .Select(r => new SelectListItem
            {
                Value = ((int)r).ToString(),
                Text = r switch
                {
                    RolUsuario.Administrador => "Administrador",
                    RolUsuario.DirigentePolitico => "Dirigente Político",
                    _ => r.ToString()
                },
                Selected = vm.Rol == (int)r
            });
    }

    private void CargarDropdownRoles(UsuarioEditViewModel vm)
    {
        vm.RolesDisponibles = Enum.GetValues<RolUsuario>()
            .Select(r => new SelectListItem
            {
                Value = ((int)r).ToString(),
                Text = r switch
                {
                    RolUsuario.Administrador => "Administrador",
                    RolUsuario.DirigentePolitico => "Dirigente Político",
                    _ => r.ToString()
                },
                Selected = vm.Rol == (int)r
            });
    }
}
