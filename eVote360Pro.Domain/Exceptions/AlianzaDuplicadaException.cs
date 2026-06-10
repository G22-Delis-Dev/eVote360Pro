namespace eVote360Pro.Domain.Exceptions;

public class AlianzaDuplicadaException : Exception
{
    public AlianzaDuplicadaException()
        : base("Ya existe una solicitud de alianza pendiente entre estos dos partidos.") { }

    public AlianzaDuplicadaException(string mensaje) : base(mensaje) { }
}