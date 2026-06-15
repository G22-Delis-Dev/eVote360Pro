using eVote360Pro.Domain.Exceptions;

namespace eVote360Pro.Domain.Rules;

public static class PartidoPoliticoRules
{
    public static void ValidarSiglasUnicas(bool existe)
    {
        if (existe)
            throw new SiglasDuplicadasException();
    }

    public static void ValidarCamposCriticosNoModificables(bool participoEnEleccion)
    {
        if (participoEnEleccion)
            throw new InvalidOperationException(
                "No se pueden modificar el nombre, las siglas ni el logo de este partido porque ya participó en una elección.");
    }

    public static void ValidarPuedeDesactivarse(bool tieneCandidatosActivos, bool tieneDirigente)
    {
        if (tieneCandidatosActivos)
            throw new InvalidOperationException(
                "No se puede desactivar este partido político porque tiene candidatos activos registrados.");

        if (tieneDirigente)
            throw new InvalidOperationException(
                "No se puede desactivar este partido político porque tiene un dirigente político asignado.");
    }
}