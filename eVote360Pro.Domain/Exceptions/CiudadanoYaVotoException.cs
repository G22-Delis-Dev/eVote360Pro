namespace eVote360Pro.Domain.Exceptions;

public class CiudadanoYaVotoException : Exception
{
    public CiudadanoYaVotoException()
        : base("Ya ha ejercido su derecho al voto.") { }

    public CiudadanoYaVotoException(string mensaje) : base(mensaje) { }
}