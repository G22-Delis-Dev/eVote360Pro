using eVote360Pro.Domain.Exceptions;

namespace eVote360Pro.Domain.Rules;

public static class UsuarioRules
{
    public static void ValidarNombreUsuarioUnico(bool existe)
    {
        if (existe)
            throw new InvalidOperationException("Ya existe un usuario registrado con este nombre de usuario.");
    }

    public static void ValidarCorreoUnico(bool existe)
    {
        if (existe)
            throw new CorreoDuplicadoException();
    }

    public static void ValidarUnicoAdministrador(int totalAdminsActivos)
    {
        if (totalAdminsActivos <= 1)
            throw new UnicoAdministradorException();
    }

    public static void ValidarNoEsMismoUsuario(int usuarioActualId, int usuarioEditandoId)
    {
        if (usuarioActualId == usuarioEditandoId)
            throw new InvalidOperationException(
                "No puede cambiar su propio rol ni desactivar su propio usuario mientras está autenticado.");
    }

    public static void ValidarRolCambiable(bool tieneDirigenteAsignado)
    {
        if (tieneDirigenteAsignado)
            throw new InvalidOperationException(
                "No se puede cambiar el rol de este usuario porque tiene un partido político asignado como dirigente.");
    }
}