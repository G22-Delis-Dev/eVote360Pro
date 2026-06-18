using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Auth;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Web.Helpers;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Web.Middlewares;

public class SesionUsuario : ISesionUsuario
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SesionUsuario(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool TieneUsuario()
    {
        UsuarioSesionViewModel? usuarioViewModel = _httpContextAccessor.HttpContext?
            .Session.Get<UsuarioSesionViewModel>("Usuario");

        return usuarioViewModel != null;
    }

    public UsuarioSesionViewModel? ObtenerUsuarioSesion()
    {
        return _httpContextAccessor.HttpContext?
            .Session.Get<UsuarioSesionViewModel>("Usuario");
    }

    public bool EsAdministrador()
    {
        UsuarioSesionViewModel? usuarioViewModel = _httpContextAccessor.HttpContext?
            .Session.Get<UsuarioSesionViewModel>("Usuario");

        if (usuarioViewModel == null)
        {
            return false;
        }

        return usuarioViewModel.Rol == RolUsuario.Administrador;
    }

    public bool EsDirigente()
    {
        UsuarioSesionViewModel? usuarioViewModel = _httpContextAccessor.HttpContext?
            .Session.Get<UsuarioSesionViewModel>("Usuario");

        if (usuarioViewModel == null)
        {
            return false;
        }

        return usuarioViewModel.Rol == RolUsuario.DirigentePolitico;
    }

    public int? ObtenerPartidoId()
    {
        return _httpContextAccessor.HttpContext?.Session.Get<UsuarioSesionViewModel>("Usuario")?.PartidoId;
    }
}
