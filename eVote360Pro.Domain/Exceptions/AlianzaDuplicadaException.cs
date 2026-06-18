namespace eVote360Pro.Domain.Exceptions;

public class AlianzaDuplicadaException : ValidacionException
{
    public AlianzaDuplicadaException()
        : base("Ya existe una alianza activa para este puesto con los mismos partidos involucrados.") { }

    public AlianzaDuplicadaException(string mensaje) : base(mensaje) { }
}