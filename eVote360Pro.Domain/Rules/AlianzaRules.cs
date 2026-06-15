using eVote360Pro.Domain.Exceptions;

namespace eVote360Pro.Domain.Rules;

public static class AlianzaRules
{
    public static void ValidarNoEsMismoPartido(int partidoAId, int partidoBId)
    {
        if (partidoAId == partidoBId)
            throw new InvalidOperationException(
                "Un partido no puede solicitar una alianza consigo mismo.");
    }

    public static void ValidarNoExisteSolicitudPendiente(bool existePendiente)
    {
        if (existePendiente)
            throw new AlianzaDuplicadaException();
    }

    public static void ValidarPuedeEliminarse(bool tieneAsignacionesAliadas)
    {
        if (tieneAsignacionesAliadas)
            throw new InvalidOperationException(
                "No se puede eliminar esta alianza porque existen candidatos aliados asignados a puestos.");
    }
}