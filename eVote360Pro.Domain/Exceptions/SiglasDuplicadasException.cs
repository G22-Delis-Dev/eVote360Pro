namespace eVote360Pro.Domain.Exceptions;

public class SiglasDuplicadasException : ValidacionException
{
    public SiglasDuplicadasException()
        : base("Ya existe un partido político registrado con estas siglas.") { }

    public SiglasDuplicadasException(string mensaje) : base(mensaje) { }
}