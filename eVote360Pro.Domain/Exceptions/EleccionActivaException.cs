namespace eVote360Pro.Domain.Exceptions;

public class EleccionActivaException : ValidacionException
{
    public EleccionActivaException()
        : base("Ya existe una elección activa en el sistema.") { }

    public EleccionActivaException(string mensaje) : base(mensaje) { }
}