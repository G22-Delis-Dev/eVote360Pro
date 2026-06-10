namespace eVote360Pro.Domain.Exceptions;

public class DocumentoDuplicadoException : Exception
{
    public DocumentoDuplicadoException()
        : base("Ya existe un ciudadano registrado con este número de documento de identidad.") { }

    public DocumentoDuplicadoException(string mensaje) : base(mensaje) { }
}