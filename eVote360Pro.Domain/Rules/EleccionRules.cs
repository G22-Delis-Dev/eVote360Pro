using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Exceptions;

namespace eVote360Pro.Domain.Rules;

public static class EleccionRules
{
    public static void ValidarNoExisteEleccionActiva(bool existeEleccionActiva)
    {
        if (existeEleccionActiva)
            throw new EleccionActivaException();
    }

    public static void ValidarExisteEleccionActiva(bool existeEleccionActiva)
    {
        if (!existeEleccionActiva)
            throw new EleccionNoActivaException();
    }

    public static void ValidarPuedeActivarse(EstadoEleccion estado, bool existeOtraActiva)
    {
        if (estado != EstadoEleccion.Pendiente)
            throw new InvalidOperationException("Solo se puede activar una elección en estado Pendiente.");

        if (existeOtraActiva)
            throw new InvalidOperationException("Ya existe una elección activa. Solo puede haber una a la vez.");
    }

    public static void ValidarPuedeFinalizarse(EstadoEleccion estado)
    {
        if (estado != EstadoEleccion.Activa)
            throw new InvalidOperationException("Solo se puede finalizar una elección en estado Activa.");
    }

    public static void ValidarPuedeVerResultados(EstadoEleccion estado)
    {
        if (estado != EstadoEleccion.Finalizada)
            throw new InvalidOperationException("Los resultados solo están disponibles para elecciones finalizadas.");
    }
}