using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class AsignacionCandidatoPuestoService : GenericService<AsignacionCandidatoPuesto, AsignacionCandidatoPuestoDto>, IAsignacionCandidatoPuestoService
{
    public AsignacionCandidatoPuestoService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.AsignacionesCandidatos)
    {
    }

    //  Validar que no exista duplicado antes de crear
    public override async Task<AsignacionCandidatoPuestoDto> CrearAsync(AsignacionCandidatoPuestoDto dto)
    {
        if (dto.CandidatoId <= 0 || dto.PuestoElectivoId <= 0 || dto.PartidoPoliticoId <= 0)
        {
            throw new ValidacionException("Debe seleccionar un candidato, un puesto electivo y un partido político válidos.");
        }

        var asignacionesExistentes = await _unitOfWork.AsignacionesCandidatos
            .FindAsync(a => a.CandidatoId == dto.CandidatoId
                     && a.PuestoElectivoId == dto.PuestoElectivoId
                     && a.PartidoPoliticoId == dto.PartidoPoliticoId);

        if (asignacionesExistentes.Any())
        {
            throw new ValidacionException("Este candidato ya se encuentra asignado a este puesto electivo por este partido.");
        }

        var asignacion = _mapper.Map<AsignacionCandidatoPuesto>(dto);

        await _repository.AddAsync(asignacion);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AsignacionCandidatoPuestoDto>(asignacion);
    }

    // Validaciones específicas al actualizar
    public override async Task ActualizarAsync(int id, AsignacionCandidatoPuestoDto dto)
    {
        var asignacionExistente = await _repository.GetByIdAsync(id);
        if (asignacionExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(AsignacionCandidatoPuesto), id);
        }

        if (dto.CandidatoId <= 0 || dto.PuestoElectivoId <= 0 || dto.PartidoPoliticoId <= 0)
        {
            throw new ValidacionException("Debe seleccionar un candidato, un puesto electivo y un partido político válidos.");
        }

        _mapper.Map(dto, asignacionExistente);

        _repository.Update(asignacionExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    // Filtrar por partido
    public async Task<IEnumerable<AsignacionCandidatoPuestoDto>> ObtenerPorPartidoAsync(int partidoId)
    {
        // Solo devuelve las asignaciones donde el partido político coincide con el del dirigente
        var asignaciones = await _unitOfWork.AsignacionesCandidatos
            .FindAsync(a => a.PartidoPoliticoId == partidoId);
        return _mapper.Map<IEnumerable<AsignacionCandidatoPuestoDto>>(asignaciones);
    }
}