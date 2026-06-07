using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface ICiudadanoRepository : IRepository<Ciudadano>
{
    Task<Ciudadano?> GetByNumeroDocumentoAsync(string numeroDocumento);
    Task<bool> ExisteNumeroDocumentoAsync(string numeroDocumento, int? excludeId = null);
    Task<bool> ExisteCorreoElectronicoAsync(string correo, int? excludeId = null);
    Task<bool> ParticipóEnEleccionAsync(int ciudadanoId);
    Task<bool> YaVotoEnEleccionAsync(int ciudadanoId, int eleccionId);
}