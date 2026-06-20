using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IEleccionService : IGenericService<EleccionDto>
{
    // Métodos específicos de Eleccion (los CRUD básicos ya los heredas de IGenericService)

    Task<IEnumerable<EleccionDto>> ObtenerOrdenadaPorFechaAsync();
    Task<IEnumerable<EleccionDto>> ObtenerPorAnioAsync(int anio);
    Task<EleccionDto?> ObtenerActivaAsync();
    Task<bool> ExisteEleccionActivaAsync();

    // Método específico de negocio que reemplaza al CrearAsync genérico
    Task CrearConPuestosAsync(EleccionDto dto, List<int> puestosIds);

    Task ActivarAsync(int id);
    Task FinalizarAsync(int id);
    Task<ResultadoEleccionDto> ObtenerResultadosAsync(int id);
    Task<IEnumerable<ResumenEleccionDto>> ObtenerResumenPorAnioAsync(int anio);
}