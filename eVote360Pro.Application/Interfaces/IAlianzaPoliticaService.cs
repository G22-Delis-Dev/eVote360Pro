using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Application.Interfaces;

public interface IAlianzaPoliticaService : IGenericService<AlianzaPoliticaDto>
{
    Task<IEnumerable<AlianzaPoliticaDto>> ObtenerPorPartidoAsync(int partidoId);

    // Método específico para el negocio de alianzas
    Task ResponderSolicitudAsync(int id, EstadoAlianza nuevoEstado);
}