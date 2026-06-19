using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Enums;

namespace eVote360Pro.Application.Interfaces;

public interface IAlianzaPoliticaService : IGenericService<AlianzaPoliticaDto>
{
    Task<IEnumerable<AlianzaPoliticaDto>> ObtenerPorPartidoAsync(int partidoId);
    Task ResponderSolicitudAsync(int id, EstadoAlianza nuevoEstado);
    Task CancelarSolicitudAsync(int id, int partidoSolicitanteId);
    Task RomperAlianzaAsync(int id, int partidoId);
}