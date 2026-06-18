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

    public async Task<bool> ParticipoEnEleccionAsync(int id)
    {
        return await _unitOfWork.PuestosElectivos.ParticipóEnEleccionAsync(id);
    }

    public override async Task<PuestoElectivoDto> CrearAsync(PuestoElectivoDto dto)
    {
        if (await _unitOfWork.PuestosElectivos.ExisteNombreAsync(dto.Nombre))
        {
            throw new Domain.Exceptions.ValidacionException($"Ya existe un puesto electivo con el nombre '{dto.Nombre}'.");
        }
        dto.Activo = true;
        return await base.CrearAsync(dto);
    }

    public override async Task ActualizarAsync(int id, PuestoElectivoDto dto)
    {
        var puestoExistente = await _unitOfWork.PuestosElectivos.GetByIdAsync(id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(PuestoElectivo), id);

        var participo = await _unitOfWork.PuestosElectivos.ParticipóEnEleccionAsync(id);
        
        if (participo && !string.Equals(puestoExistente.Nombre, dto.Nombre, StringComparison.OrdinalIgnoreCase))
        {
            throw new Domain.Exceptions.ValidacionException("El Nombre del Puesto no se puede modificar porque está incluido en una elección activa.");
        }

        if (await _unitOfWork.PuestosElectivos.ExisteNombreAsync(dto.Nombre, id))
        {
            throw new Domain.Exceptions.ValidacionException($"Ya existe un puesto electivo con el nombre '{dto.Nombre}'.");
        }

        await base.ActualizarAsync(id, dto);
    }

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