namespace eVote360Pro.Domain.Exceptions;

public class UnicoAdministradorException : Exception
{
    public UnicoAdministradorException()
        : base("No se puede modificar este usuario porque es el único administrador activo del sistema.") { }

    public UnicoAdministradorException(string mensaje) : base(mensaje) { }
}