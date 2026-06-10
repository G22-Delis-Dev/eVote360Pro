namespace eVote360Pro.Domain.Exceptions;

public class CiudadanoInactivoException : Exception
{
    public CiudadanoInactivoException()
        : base("Este ciudadano se encuentra inactivo y no puede participar en el proceso de votación.") { }

    public CiudadanoInactivoException(string mensaje) : base(mensaje) { }
}