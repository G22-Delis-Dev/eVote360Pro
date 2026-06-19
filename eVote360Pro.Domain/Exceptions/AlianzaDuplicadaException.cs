namespace eVote360Pro.Domain.Exceptions;

public class AlianzaDuplicadaException : ValidacionException
{
    public AlianzaDuplicadaException()
        : base("Ya existe una solicitud de alianza pendiente entre estos partidos.") { }

    public AlianzaDuplicadaException(string mensaje) : base(mensaje) { }
}