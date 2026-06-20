namespace eVote360Pro.Domain.Rules;

public static class CandidatoRules
{
    public static void ValidarCamposCriticosNoModificables(bool participoEnEleccion)
    {
        if (participoEnEleccion)
            throw new InvalidOperationException(
                "No se pueden modificar el nombre, apellido ni foto de este candidato porque ya participó en una elección.");
    }

    public static void ValidarPuedeDesactivarse(bool estaAsignadoAPuesto)
    {
        if (estaAsignadoAPuesto)
            throw new InvalidOperationException(
                "No se puede desactivar este candidato porque está asignado a un puesto electivo.");
    }
}