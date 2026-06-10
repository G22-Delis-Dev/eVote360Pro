using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface ICodigoVerificacionRepository : IRepository<CodigoVerificacion>
{
    Task<CodigoVerificacion?> GetCodigoVigenteAsync(int ciudadanoId, int eleccionId, string codigo);
    Task InvalidarCodigosAnterioresAsync(int ciudadanoId, int eleccionId);
}