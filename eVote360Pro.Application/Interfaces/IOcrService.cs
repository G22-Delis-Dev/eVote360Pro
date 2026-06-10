namespace eVote360Pro.Application.Interfaces;

public interface IOcrService
{
    Task<string?> ExtraerNumeroDocumentoAsync(Stream imagenStream);
}