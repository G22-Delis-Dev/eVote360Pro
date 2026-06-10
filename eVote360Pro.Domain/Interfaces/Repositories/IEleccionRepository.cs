using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Domain.Interfaces.Repositories;

public interface IEleccionRepository : IRepository<Eleccion>
{
    Task<Eleccion?> GetEleccionActivaAsync();
    Task<bool> ExisteEleccionActivaAsync();
    Task<IEnumerable<Eleccion>> GetOrdenadaPorFechaDescAsync();
    Task<IEnumerable<Eleccion>> GetByAnioAsync(int anio);
}