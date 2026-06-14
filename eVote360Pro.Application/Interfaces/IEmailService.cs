namespace eVote360Pro.Application.Interfaces;

public interface IEmailService
{
    // Recibe el cuerpo ya formateado (HTML o texto)
    Task EnviarAsync(string destinatario, string asunto, string cuerpo);
}