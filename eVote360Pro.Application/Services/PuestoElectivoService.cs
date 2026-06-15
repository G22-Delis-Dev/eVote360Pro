using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class PuestoElectivoService : GenericService<PuestoElectivo, PuestoElectivoDto>, IPuestoElectivoService
{
    public PuestoElectivoService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.PuestosElectivos) { }

    // Obtiene solo los puestos que están activos para los selectores o vistas
    public async Task<IEnumerable<PuestoElectivoDto>> ObtenerActivosAsync()
    {
        var puestos = await _unitOfWork.PuestosElectivos.GetAllAsync();
        return _mapper.Map<IEnumerable<PuestoElectivoDto>>(puestos.Where(p => p.Activo));
    }

    // Método para activar/desactivar un puesto
    public async Task CambiarEstadoAsync(int id)
    {
        var puesto = await _unitOfWork.PuestosElectivos.GetByIdAsync(id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(PuestoElectivo), id);

        puesto.Activo = !puesto.Activo;
        puesto.FechaModificacion = DateTime.UtcNow;

        _unitOfWork.PuestosElectivos.Update(puesto);
        await _unitOfWork.SaveChangesAsync();
    }
}