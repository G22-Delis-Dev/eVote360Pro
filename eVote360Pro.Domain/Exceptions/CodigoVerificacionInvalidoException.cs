namespace eVote360Pro.Domain.Exceptions;

public class CodigoVerificacionInvalidoException : Exception
{
    public CodigoVerificacionInvalidoException()
        : base("El código ingresado es incorrecto, ya fue utilizado o ha expirado.") { }

    public CodigoVerificacionInvalidoException(string mensaje) : base(mensaje) { }
}