using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class AsignacionCandidatoPuestoService : IAsignacionCandidatoPuestoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AsignacionCandidatoPuestoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AsignacionCandidatoPuestoDto>> ObtenerTodasAsync()
    {
        var asignaciones = await _unitOfWork.AsignacionesCandidatos.GetAllAsync();
        return _mapper.Map<IEnumerable<AsignacionCandidatoPuestoDto>>(asignaciones);
    }

    public async Task<AsignacionCandidatoPuestoDto?> ObtenerPorIdAsync(int id)
    {
        var asignacion = await _unitOfWork.AsignacionesCandidatos.GetByIdAsync(id);
        return _mapper.Map<AsignacionCandidatoPuestoDto>(asignacion);
    }

    public async Task<AsignacionCandidatoPuestoDto> CrearAsync(AsignacionCandidatoPuestoDto dto)
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

        await _unitOfWork.AsignacionesCandidatos.AddAsync(asignacion);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AsignacionCandidatoPuestoDto>(asignacion);
    }

    public async Task ActualizarAsync(int id, AsignacionCandidatoPuestoDto dto)
    {
        var asignacionExistente = await _unitOfWork.AsignacionesCandidatos.GetByIdAsync(id);
        if (asignacionExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(AsignacionCandidatoPuesto), id);
        }

        if (dto.CandidatoId <= 0 || dto.PuestoElectivoId <= 0 || dto.PartidoPoliticoId <= 0)
        {
            throw new ValidacionException("Debe seleccionar un candidato, un puesto electivo y un partido político válidos.");
        }

        _mapper.Map(dto, asignacionExistente);

        _unitOfWork.AsignacionesCandidatos.Update(asignacionExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var asignacion = await _unitOfWork.AsignacionesCandidatos.GetByIdAsync(id);
        if (asignacion != null)
        {
            _unitOfWork.AsignacionesCandidatos.Remove(asignacion);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new RegistroNoEncontradoException(nameof(AsignacionCandidatoPuesto), id);
        }
    }
}