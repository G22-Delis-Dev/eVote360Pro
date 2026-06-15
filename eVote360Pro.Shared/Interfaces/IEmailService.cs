namespace eVote360Pro.Shared.Interfaces;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml);
    Task EnviarCodigoVerificacionAsync(string destinatario, string nombreCiudadano, string codigo);
    Task EnviarResumenVotacionAsync(string destinatario, string nombreCiudadano, object resumenDto);
}