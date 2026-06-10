namespace eVote360Pro.Domain.Exceptions;

public class CorreoDuplicadoException : Exception
{
    public CorreoDuplicadoException()
        : base("Ya existe un registro con este correo electrónico.") { }

    public CorreoDuplicadoException(string mensaje) : base(mensaje) { }
}