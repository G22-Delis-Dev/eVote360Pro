using eVote360Pro.Application.ViewModels.Auth;

namespace eVote360Pro.Application.Interfaces;

public interface ISesionUsuario
{
    UsuarioSesionViewModel? ObtenerUsuarioSesion();
    bool TieneUsuario();
    bool EsAdministrador();
    bool EsDirigente();
    int? ObtenerPartidoId();
}
