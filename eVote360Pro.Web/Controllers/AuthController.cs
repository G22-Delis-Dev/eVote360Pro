using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Auth;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly ISesionUsuario _sesionUsuario;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IUsuarioService usuarioService, ISesionUsuario sesionUsuario, IUnitOfWork unitOfWork)
    {
        _usuarioService = usuarioService;
        _sesionUsuario = sesionUsuario;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (_sesionUsuario.TieneUsuario())
        {
            var usuarioSesion = _sesionUsuario.ObtenerUsuarioSesion();
            if (usuarioSesion != null)
            {
                return usuarioSesion.Rol switch
                {
                    RolUsuario.Administrador => RedirectToAction("Index", "HomeAdmin"),
                    RolUsuario.DirigentePolitico => RedirectToAction("Index", "HomeDirigente"),
                    _ => RedirectToAction("Login", "Auth")
                };
            }
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (_sesionUsuario.TieneUsuario())
        {
            var sesionActual = _sesionUsuario.ObtenerUsuarioSesion();
            if (sesionActual != null)
            {
                return sesionActual.Rol switch
                {
                    RolUsuario.Administrador => RedirectToAction("Index", "HomeAdmin"),
                    RolUsuario.DirigentePolitico => RedirectToAction("Index", "HomeDirigente"),
                    _ => RedirectToAction("Login", "Auth")
                };
            }
        }

        if (!ModelState.IsValid)
        {
            vm.Password = string.Empty;
            return View(vm);
        }

        var usuarioDto = await _usuarioService.ValidarCredencialesAsync(vm.NombreUsuario, vm.Password);

        if (usuarioDto != null)
        {
            var usuarioSesion = new UsuarioSesionViewModel
            {
                Id = usuarioDto.Id,
                Nombre = usuarioDto.Nombre,
                Apellido = usuarioDto.Apellido,
                CorreoElectronico = usuarioDto.CorreoElectronico,
                NombreUsuario = usuarioDto.NombreUsuario,
                Rol = usuarioDto.Rol
            };

            if (usuarioSesion.Rol == RolUsuario.DirigentePolitico)
            {
                var asignacion = await _unitOfWork.AsignacionesDirigentes.GetByUsuarioAsync(usuarioDto.Id);
                usuarioSesion.PartidoId = asignacion?.PartidoPoliticoId;
            }

            if (usuarioSesion.Rol == RolUsuario.Administrador)
            {
                HttpContext.Session.Set("Usuario", usuarioSesion);
                return RedirectToAction("Index", "HomeAdmin");
            }
            else if (usuarioSesion.Rol == RolUsuario.DirigentePolitico)
            {
                if (usuarioSesion.PartidoId == null || usuarioSesion.PartidoId <= 0)
                {
                    ModelState.AddModelError(string.Empty, "No tiene un partido político asignado, por lo tanto no puede iniciar sesión. Por favor, póngase en contacto con un administrador.");
                    vm.Password = string.Empty;
                    return View(vm);
                }
                
                HttpContext.Session.Set("Usuario", usuarioSesion);
                return RedirectToAction("Index", "HomeDirigente");
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas o usuario inactivo.");
        }

        vm.Password = string.Empty;
        return View(vm);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("Usuario");
        return RedirectToAction("Login", "Auth");
    }

    public IActionResult AccesoDenegado()
    {
        if (_sesionUsuario.TieneUsuario())
        {
            return View();
        }

        return RedirectToAction("Login", "Auth");
    }
}
