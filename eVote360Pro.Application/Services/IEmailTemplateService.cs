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

        return html.Replace("{{Nombre}}", nombreCiudadano)
                   .Replace("{{NombreEleccion}}", resumen.NombreEleccion);
        // Puedes agregar más reemplazos según los campos de tu DTO
    }
}