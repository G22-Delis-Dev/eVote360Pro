using Tesseract;

namespace eVote360Pro.Infrastructure.Services;

public interface IOcrService
{
    Task<string?> ExtraerNumeroDocumentoAsync(Stream imagenStream);
}

public class OcrService : IOcrService
{
    private readonly string _tessDataPath;

    // tessDataPath apunta a la carpeta donde están los archivos .traineddata
    // Por defecto Tesseract busca en ./tessdata
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

    // Intenta extraer el número de documento del texto OCR.
    // Las cédulas dominicanas tienen el formato: 000-0000000-0
    private static string? ExtraerNumeroDocumento(string texto)
    {
        // Limpia el texto
        var lineas = texto
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l));

        foreach (var linea in lineas)
        {
            // Patrón con guiones: 000-0000000-0
            var conGuiones = System.Text.RegularExpressions.Regex.Match(
                linea, @"\b\d{3}-\d{7}-\d{1}\b");

            if (conGuiones.Success)
            {
                // Retorna sin guiones para comparar con NumeroDocumento del sistema
                return conGuiones.Value.Replace("-", "");
            }

            // Patrón sin guiones: 00112345678 (11 dígitos)
            var sinGuiones = System.Text.RegularExpressions.Regex.Match(
                linea, @"\b\d{11}\b");

            if (sinGuiones.Success)
                return sinGuiones.Value;
        }

        return null;
    }
}