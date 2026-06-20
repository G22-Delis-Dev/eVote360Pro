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
    private readonly IEleccionService _eleccionService;
    private readonly ICiudadanoService _ciudadanoService;

    public UsuariosController(
        IUsuarioService usuarioService,
        IMapper mapper,
        eVote360Pro.Application.Interfaces.ISesionUsuario sesionUsuario,
        IEleccionService eleccionService,
        ICiudadanoService ciudadanoService)
    {
        _usuarioService = usuarioService;
        _mapper = mapper;
        _sesionUsuario = sesionUsuario;
        _eleccionService = eleccionService;
        _ciudadanoService = ciudadanoService;
    }

    // TODO: Reemplazar con el ID real del administrador autenticado
    // cuando se implemente el sistema de autenticación (Claims/Session).
    private int ObtenerUsuarioIdActual() => _sesionUsuario.ObtenerUsuarioSesion()?.Id ?? 0;

    public async Task<IActionResult> Index(string? filtro)
    {
        var dtos = await _usuarioService.ObtenerListaAsync();
        var items = _mapper.Map<IEnumerable<UsuarioItemViewModel>>(dtos);

        if (!string.IsNullOrEmpty(filtro))
        {
            var terminos = filtro.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            items = items.Where(u => terminos.All(t => 
                (u.NombreUsuario?.Contains(t, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.CiudadanoNombreCompleto?.Contains(t, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.Rol?.Contains(t, StringComparison.OrdinalIgnoreCase) ?? false)));
        }

        var vm = new UsuarioListViewModel
        {
            Usuarios = items,
            Filtro = filtro
        };

        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();
        var vm = new UsuarioCreateViewModel();
        await CargarDropdownsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel vm)
    {
        // El formulario no envía Nombre, Apellido, ni Correo porque se toman del ciudadano.
        // Removemos los errores de validación de esos campos si se seleccionó un ciudadano.
        if (vm.CiudadanoId.HasValue)
        {
            ModelState.Remove(nameof(vm.Nombre));
            ModelState.Remove(nameof(vm.Apellido));
            ModelState.Remove(nameof(vm.CorreoElectronico));
        }

        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync(vm);
            return View(vm);
        }

        try
        {
            // Completamos los datos faltantes buscando el ciudadano
            if (vm.CiudadanoId.HasValue)
            {
                var ciudadano = await _ciudadanoService.ObtenerPorIdAsync(vm.CiudadanoId.Value);
                if (ciudadano != null)
                {
                    vm.Nombre = ciudadano.Nombre;
                    vm.Apellido = ciudadano.Apellido;
                    vm.CorreoElectronico = ciudadano.CorreoElectronico;
                }
            }

            var dto = _mapper.Map<UsuarioDto>(vm);
            await _usuarioService.CrearAsync(dto, vm.Password);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarDropdownsAsync(vm);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _usuarioService.ObtenerPorIdAsync(id);
        if (dto == null) return NotFound();

        ViewBag.HayEleccionActiva = await _eleccionService.ExisteEleccionActivaAsync();

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

    private async Task CargarDropdownsAsync(UsuarioCreateViewModel vm)
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

        var ciudadanos = await _ciudadanoService.ObtenerListaAsync();
        // Solo ciudadanos que no tienen usuario (esto deberia venir del service, pero lo filtramos aqui o dejamos que falle validacion,
        // Asumiendo que el service retorna todos, podríamos filtrarlos o solo cargarlos).
        vm.CiudadanosDisponibles = ciudadanos.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.Nombre} {c.Apellido} - {c.NumeroDocumento}"
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
