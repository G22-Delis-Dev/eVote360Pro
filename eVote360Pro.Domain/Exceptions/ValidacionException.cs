namespace eVote360Pro.Domain.Exceptions;

public class ValidacionException : Exception
{
    public ValidacionException(string mensaje) : base(mensaje)
    {
    }
}