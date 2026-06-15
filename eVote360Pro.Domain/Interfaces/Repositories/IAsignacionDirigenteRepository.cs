using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IAsignacionDirigenteRepository : IRepository<AsignacionDirigente>
{
    Task<AsignacionDirigente?> GetByUsuarioAsync(int usuarioId);
    Task<AsignacionDirigente?> GetByPartidoAsync(int partidoId);

    // Métodos necesarios para la lógica del servicio
    Task<bool> DirigenteTienePartidoAsync(int usuarioId);
    Task<bool> PartidoTieneDirigenteAsync(int partidoId);
}