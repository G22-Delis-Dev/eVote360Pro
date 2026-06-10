namespace eVote360Pro.Domain.Exceptions;

public class EleccionNoActivaException : Exception
{
    public EleccionNoActivaException()
        : base("No hay ningún proceso electoral activo en estos momentos.") { }

    public EleccionNoActivaException(string mensaje) : base(mensaje) { }
}