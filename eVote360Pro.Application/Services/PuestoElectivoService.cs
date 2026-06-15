using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class PuestoElectivoService : GenericService<PuestoElectivo, PuestoElectivoDto>, IPuestoElectivoService
{
    public PuestoElectivoService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.PuestosElectivos)
    {
    }

    // Hereda todas las operaciones CRUD de GenericService.
    // No necesita sobrescribir nada porque no tiene lógica de negocio especial.
    // Agregar aquí métodos específicos del módulo cuando sea necesario.
}