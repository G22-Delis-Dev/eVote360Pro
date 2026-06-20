namespace eVote360Pro.Domain.Rules;

public static class AsignacionCandidatoRules
{
    public static void ValidarCandidatoSinAsignacionEnPartido(bool tieneAsignacion)
    {
        if (tieneAsignacion)
            throw new InvalidOperationException(
                "Este candidato ya está asignado a un puesto dentro de este partido.");
    }

    public static void ValidarPuestoSinAsignacionEnPartido(bool tieneAsignacion)
    {
        if (tieneAsignacion)
            throw new InvalidOperationException(
                "Este puesto ya tiene un candidato asignado dentro de este partido.");
    }

    public static void ValidarCandidatoAliadoMismoPuesto(int puestoOrigenId, int puestoSolicitadoId)
    {
        if (puestoOrigenId != puestoSolicitadoId)
            throw new InvalidOperationException(
                "Un candidato aliado solo puede aspirar al mismo puesto que tiene en su partido de origen.");
    }
}