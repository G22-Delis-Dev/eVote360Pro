namespace eVote360Pro.Domain.Interfaces;

public interface IOcrService
{
    Task<string?> ExtraerNumeroDocumentoAsync(Stream imagenStream);
}