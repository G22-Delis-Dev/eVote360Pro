using eVote360Pro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360Pro.Web.Filters;

public class ValidarSesionAttribute : ActionFilterAttribute
{
    private readonly string? _rolRequerido;

    public ValidarSesionAttribute(string? rolRequerido = null)
    {
        _rolRequerido = rolRequerido;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Evitar que el navegador cachee páginas autenticadas
        var response = context.HttpContext.Response;
        response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        response.Headers["Pragma"] = "no-cache";
        response.Headers["Expires"] = "0";

        var sesionUsuario = context.HttpContext.RequestServices.GetService<ISesionUsuario>();

        if (sesionUsuario == null || !sesionUsuario.TieneUsuario())
        {
            context.Result = new RedirectToRouteResult(new { controller = "Auth", action = "Login" });
            return;
        }

        if (_rolRequerido != null)
        {
            if (_rolRequerido == "Administrador" && !sesionUsuario.EsAdministrador())
            {
                context.Result = new RedirectToRouteResult(new { controller = "Auth", action = "AccesoDenegado" });
                return;
            }
            if (_rolRequerido == "DirigentePolitico" && !sesionUsuario.EsDirigente())
            {
                context.Result = new RedirectToRouteResult(new { controller = "Auth", action = "AccesoDenegado" });
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
