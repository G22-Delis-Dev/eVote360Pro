using eVote360Pro.Application.Interfaces;
using Tesseract;

namespace eVote360Pro.Infrastructure.Services;

// Infrastructure no implementa IEmailService ni IOcrService directamente.
// La conexión con IOcrService se hace en Program.cs via DI.
public class OcrService : IOcrService
{
    private readonly string _tessDataPath;

    public OcrService(string tessDataPath = "tessdata")
    {
        _tessDataPath = tessDataPath;
    }

    public Task<string?> ExtraerNumeroDocumentoAsync(Stream imagenStream)
    {
        try
        {
            using var engine = new TesseractEngine(_tessDataPath, "spa", EngineMode.Default);
            using var memoryStream = new MemoryStream();

            imagenStream.CopyTo(memoryStream);
            var imageBytes = memoryStream.ToArray();

            using var img = Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(img);

            var textoExtraido = page.GetText();

            if (string.IsNullOrWhiteSpace(textoExtraido))
                return Task.FromResult<string?>(null);

            var numeroDocumento = ExtraerNumeroDocumento(textoExtraido);
            return Task.FromResult(numeroDocumento);
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }

    private static string? ExtraerNumeroDocumento(string texto)
    {
        var lineas = texto
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l));

        foreach (var linea in lineas)
        {
            // Formato con guiones: 000-0000000-0
            var conGuiones = System.Text.RegularExpressions.Regex.Match(
                linea, @"\b\d{3}[-\s.]\d{7}[-\s.]\d{1}\b");

            if (conGuiones.Success)
            {
                // Normalizar: quitar guiones, espacios y puntos
                return System.Text.RegularExpressions.Regex.Replace(conGuiones.Value, @"[-\s.]", "");
            }

            // Formato sin separadores: 11 dígitos consecutivos
            var sinGuiones = System.Text.RegularExpressions.Regex.Match(
                linea, @"\b\d{11}\b");

            if (sinGuiones.Success)
                return sinGuiones.Value;

            // Formato permisivo: 3 dígitos, algo, 7 dígitos, algo, 1 dígito (OCR impreciso)
            var permisivo = System.Text.RegularExpressions.Regex.Match(
                linea, @"(\d{3})\D{0,2}(\d{7})\D{0,2}(\d{1})");

            if (permisivo.Success)
                return permisivo.Groups[1].Value + permisivo.Groups[2].Value + permisivo.Groups[3].Value;
        }

        return null;
    }
}