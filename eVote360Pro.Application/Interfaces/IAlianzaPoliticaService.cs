using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Application.Interfaces;

public interface IAlianzaPoliticaService
{
    Task<IEnumerable<AlianzaPoliticaDto>> ObtenerTodasAsync();
    Task<IEnumerable<AlianzaPoliticaDto>> ObtenerPorPartidoAsync(int partidoId);
    Task<AlianzaPoliticaDto?> ObtenerPorIdAsync(int id);
    Task<AlianzaPoliticaDto> CrearAsync(AlianzaPoliticaDto dto);
    Task EliminarAsync(int id);

    // Método específico para el negocio de alianzas
    Task ResponderSolicitudAsync(int id, EstadoAlianza nuevoEstado);
}