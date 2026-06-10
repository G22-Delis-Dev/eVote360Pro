using Tesseract;
using eVote360Pro.Domain.Interfaces;

namespace eVote360Pro.Infrastructure.Services;

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
            var conGuiones = System.Text.RegularExpressions.Regex.Match(
                linea, @"\b\d{3}-\d{7}-\d{1}\b");

            if (conGuiones.Success)
                return conGuiones.Value.Replace("-", "");

            var sinGuiones = System.Text.RegularExpressions.Regex.Match(
                linea, @"\b\d{11}\b");

            if (sinGuiones.Success)
                return sinGuiones.Value;
        }

        return null;
    }
}