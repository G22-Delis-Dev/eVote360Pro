using eVote360Pro.Domain.Exceptions;

namespace eVote360Pro.Domain.Rules;

public static class CiudadanoRules
{
    public static void ValidarActivo(bool activo)
    {
        if (!activo)
            throw new CiudadanoInactivoException();
    }

    public static void ValidarNoHaVotado(bool yaVoto)
    {
        if (yaVoto)
            throw new CiudadanoYaVotoException();
    }

    public static void ValidarDocumentoUnico(bool existeDocumento)
    {
        if (existeDocumento)
            throw new DocumentoDuplicadoException();
    }

    public static void ValidarCorreoUnico(bool existeCorreo)
    {
        if (existeCorreo)
            throw new CorreoDuplicadoException();
    }

    public static void ValidarDocumentoNoModificable(bool participoEnEleccion)
    {
        if (participoEnEleccion)
            throw new InvalidOperationException(
                "No se puede modificar el número de documento de identidad de este ciudadano porque ya participó en una elección.");
    }
}