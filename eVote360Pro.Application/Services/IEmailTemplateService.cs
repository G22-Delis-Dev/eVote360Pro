using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using System.Reflection;

namespace eVote360Pro.Application.Services;

public class EmailTemplateService : IEmailTemplateService
{
    // Método para obtener el contenido del archivo HTML desde los recursos
    private string ObtenerPlantilla(string nombreArchivo)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = $"eVote360Pro.Application.Templates.{nombreArchivo}";

        using var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null) throw new FileNotFoundException($"No se encontró la plantilla: {resourcePath}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public string GenerarCodigoVerificacionHtml(string nombreCiudadano, string codigo)
    {
        var html = ObtenerPlantilla("CodigoVerificacion.html");

        return html.Replace("{{Nombre}}", nombreCiudadano)
                   .Replace("{{Codigo}}", codigo);
    }

    public string GenerarResumenVotacionHtml(string nombreCiudadano, ResumenVotacionDto resumen)
    {
        var html = ObtenerPlantilla("ResumenVotacion.html");

        // Construir tabla de votos
        var filasVotos = new System.Text.StringBuilder();
        foreach (var voto in resumen.Votos)
        {
            filasVotos.AppendLine($@"
                <tr>
                    <td style=""padding: 10px 12px; border-bottom: 1px solid #e8e8e8; font-weight: 600; color: #333;"">{System.Net.WebUtility.HtmlEncode(voto.Puesto)}</td>
                    <td style=""padding: 10px 12px; border-bottom: 1px solid #e8e8e8; color: #444;"">{System.Net.WebUtility.HtmlEncode(voto.Candidato)}</td>
                    <td style=""padding: 10px 12px; border-bottom: 1px solid #e8e8e8; color: #666;"">{System.Net.WebUtility.HtmlEncode(voto.Partido)}</td>
                </tr>");
        }

        return html
            .Replace("{{Nombre}}", System.Net.WebUtility.HtmlEncode(nombreCiudadano))
            .Replace("{{NombreEleccion}}", System.Net.WebUtility.HtmlEncode(resumen.NombreEleccion))
            .Replace("{{FechaVotacion}}", resumen.FechaEleccion.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")))
            .Replace("{{FilasVotos}}", filasVotos.ToString());
    }
}