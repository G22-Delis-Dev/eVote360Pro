namespace eVote360Pro.Domain.Exceptions;

public class UnicoAdministradorException : ValidacionException
{
    public UnicoAdministradorException()
        : base("No se puede desactivar ni eliminar el único administrador del sistema.") { }

    public UnicoAdministradorException(string mensaje) : base(mensaje) { }
}