namespace eVote360Pro.Shared.Interfaces;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml);
}