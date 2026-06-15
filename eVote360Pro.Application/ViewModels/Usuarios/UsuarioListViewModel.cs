namespace eVote360Pro.Application.ViewModels.Usuarios;

public class UsuarioListViewModel
{
    public IEnumerable<UsuarioItemViewModel> Usuarios { get; set; } = new List<UsuarioItemViewModel>();
}

public class UsuarioItemViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
}