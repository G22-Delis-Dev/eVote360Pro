namespace eVote360Pro.Domain.Exceptions;

public class DocumentoDuplicadoException : ValidacionException
{
    public DocumentoDuplicadoException()
        : base("Ya existe un ciudadano registrado con este número de documento de identidad.") { }

    public DocumentoDuplicadoException(string mensaje) : base(mensaje) { }
}