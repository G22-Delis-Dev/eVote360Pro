namespace eVote360Pro.Domain.Settings;
public class EmailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Puerto { get; set; } = 587;
    public string CorreoRemitente { get; set; } = string.Empty;
    public string NombreRemitente { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}