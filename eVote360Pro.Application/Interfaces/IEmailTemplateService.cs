using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IEmailTemplateService
{
    string GenerarCodigoVerificacionHtml(string nombreCiudadano, string codigo);
    string GenerarResumenVotacionHtml(string nombreCiudadano, ResumenVotacionDto resumen);
}