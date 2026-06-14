namespace eVote360Pro.Domain.Exceptions;

public class RegistroNoEncontradoException : Exception
{
    public RegistroNoEncontradoException(string entidad, object id)
        : base($"El registro de \"{entidad}\" con el identificador ({id}) no fue encontrado en la base de datos.")
    {
    }
}