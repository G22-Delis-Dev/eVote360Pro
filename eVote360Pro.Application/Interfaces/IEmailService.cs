using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IEmailService
{
    Task EnviarCodigoVerificacionAsync(string destinatario, string nombreCiudadano, string codigo);
    Task EnviarResumenVotacionAsync(string destinatario, string nombreCiudadano, ResumenVotacionDto resumen);
}