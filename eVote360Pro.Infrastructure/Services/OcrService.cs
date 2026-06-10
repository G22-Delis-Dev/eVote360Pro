using Tesseract;
using eVote360Pro.Application.Interfaces;

namespace eVote360Pro.Infrastructure.Services;

public class OcrService : IOcrService
{
    private readonly TesseractEngine _engine;

    public OcrService(string tessDataPath = "tessdata")
    {
        // El motor es pesado, lo instanciamos una vez al crear el servicio
        _engine = new TesseractEngine(tessDataPath, "spa", EngineMode.Default);
    }

    public async Task<string?> ExtraerNumeroDocumentoAsync(Stream imagenStream)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await imagenStream.CopyToAsync(memoryStream);
            
            using var img = Pix.LoadFromMemory(memoryStream.ToArray());
            using var page = _engine.Process(img);
            
            var textoExtraido = page.GetText();

            return !string.IsNullOrWhiteSpace(textoExtraido) 
                ? ExtraerNumeroDocumento(textoExtraido) 
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static string? ExtraerNumeroDocumento(string texto)
    {
        var lineas = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                          .Select(l => l.Trim())
                          .Where(l => !string.IsNullOrWhiteSpace(l));

        foreach (var linea in lineas)
        {
            // Busca formato 000-0000000-0
            var conGuiones = System.Text.RegularExpressions.Regex.Match(linea, @"\b\d{3}-\d{7}-\d{1}\b");
            if (conGuiones.Success) return conGuiones.Value.Replace("-", "");

            // Busca formato 00000000000
            var sinGuiones = System.Text.RegularExpressions.Regex.Match(linea, @"\b\d{11}\b");
            if (sinGuiones.Success) return sinGuiones.Value;
        }
        return null;
    }
}