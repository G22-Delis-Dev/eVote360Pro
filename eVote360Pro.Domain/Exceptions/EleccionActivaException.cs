namespace eVote360Pro.Domain.Exceptions;

public class EleccionActivaException : Exception
{
    public EleccionActivaException()
        : base("No se puede realizar esta acción mientras exista una elección activa.") { }

    public EleccionActivaException(string mensaje) : base(mensaje) { }
}