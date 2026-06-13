using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Services;

public class EmailTemplateService : IEmailTemplateService
{
    public string GenerarCodigoVerificacionHtml(string nombreCiudadano, string codigo)
    {
        return $@"<html><body><h2>Verificación</h2><p>Hola {nombreCiudadano}, tu código es: {codigo}</p></body></html>";
    }

    public string GenerarResumenVotacionHtml(string nombreCiudadano, ResumenVotacionDto resumen)
    {
        var filas = string.Join("", resumen.Votos.Select(v => $"<tr><td>{v.Puesto}</td><td>{v.Candidato}</td></tr>"));
        return $@"<html><body><h2>Resumen de {resumen.NombreEleccion}</h2><table>{filas}</table></body></html>";
    }
}