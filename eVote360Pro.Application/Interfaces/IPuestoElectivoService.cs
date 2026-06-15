using eVote360Pro.Application.DTOs;

namespace eVote360Pro.Application.Interfaces;

public interface IPuestoElectivoService : IGenericService<PuestoElectivoDto>
{
    // Hereda todas las operaciones CRUD de IGenericService.
    // Agregar aquí métodos específicos del módulo cuando sea necesario.
}