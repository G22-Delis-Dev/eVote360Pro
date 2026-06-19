using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

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
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        if (dto.CandidatoId <= 0 || dto.PuestoElectivoId <= 0 || dto.PartidoPoliticoId <= 0)
        {
            throw new ValidacionException("Debe seleccionar un candidato, un puesto electivo y un partido político válidos.");
        }

        // Si es un candidato aliado, validar que aspira al mismo puesto que tiene en su partido de origen
        if (dto.EsAliado)
        {
            // Buscar en qué puesto está asignado este candidato en su partido de origen
            var asignacionOrigen = await _unitOfWork.AsignacionesCandidatos
                .FindAsync(a => a.CandidatoId == dto.CandidatoId && !a.EsAliado && a.Activo);

            var asignacionOriginal = asignacionOrigen.FirstOrDefault();

            if (asignacionOriginal == null)
                throw new ValidacionException("El candidato aliado no tiene un puesto asignado en su partido de origen.");

            AsignacionCandidatoRules.ValidarCandidatoAliadoMismoPuesto(
                asignacionOriginal.PuestoElectivoId,
                dto.PuestoElectivoId);
        }

        // Validar reglas de negocio usando el repositorio
        var tieneAsignacionCandidato = await _unitOfWork.AsignacionesCandidatos.CandidatoTieneAsignacionEnPartidoAsync(dto.CandidatoId, dto.PartidoPoliticoId);
        if (tieneAsignacionCandidato)
        {
            throw new ValidacionException("Este candidato ya está asignado a un puesto dentro de este partido.");
        }

        var tieneAsignacionPuesto = await _unitOfWork.AsignacionesCandidatos.PuestoTieneAsignacionEnPartidoAsync(dto.PuestoElectivoId, dto.PartidoPoliticoId);
        if (tieneAsignacionPuesto)
        {
            throw new ValidacionException("Este puesto ya tiene un candidato asignado dentro de este partido.");
        }

        // Validaciones propias del partido (para evitar 2 candidatos al mismo puesto o 1 candidato a 2 puestos)
        AsignacionCandidatoRules.ValidarCandidatoSinAsignacionEnPartido(await _unitOfWork.AsignacionesCandidatos.CandidatoTieneAsignacionEnPartidoAsync(dto.CandidatoId, dto.PartidoPoliticoId));
        AsignacionCandidatoRules.ValidarPuestoSinAsignacionEnPartido(await _unitOfWork.AsignacionesCandidatos.PuestoTieneAsignacionEnPartidoAsync(dto.PuestoElectivoId, dto.PartidoPoliticoId));

        var asignacion = _mapper.Map<AsignacionCandidatoPuesto>(dto);

        await _repository.AddAsync(asignacion);
        await _unitOfWork.SaveChangesAsync();

        // Mapear de vuelta incluyendo las relaciones
        var entidadGuardada = await _unitOfWork.AsignacionesCandidatos.GetByPartidoAsync(dto.PartidoPoliticoId);
        var asignacionGuardada = entidadGuardada.FirstOrDefault(a => a.Id == asignacion.Id);

        return _mapper.Map<AsignacionCandidatoPuestoDto>(asignacionGuardada ?? asignacion);
    }

    // Validaciones específicas al actualizar
    public override async Task ActualizarAsync(int id, AsignacionCandidatoPuestoDto dto)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var asignacionExistente = await _repository.GetByIdAsync(id);
        if (asignacionExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(AsignacionCandidatoPuesto), id);
        }

        if (dto.CandidatoId <= 0 || dto.PuestoElectivoId <= 0 || dto.PartidoPoliticoId <= 0)
        {
            throw new ValidacionException("Debe seleccionar un candidato, un puesto electivo y un partido político válidos.");
        }

        // Si cambió de candidato, validar que el nuevo candidato no tenga otra asignación
        if (asignacionExistente.CandidatoId != dto.CandidatoId)
        {
            var tieneAsignacionCandidato = await _unitOfWork.AsignacionesCandidatos.CandidatoTieneAsignacionEnPartidoAsync(dto.CandidatoId, dto.PartidoPoliticoId);
            if (tieneAsignacionCandidato)
            {
                throw new ValidacionException("Este candidato ya está asignado a un puesto dentro de este partido.");
            }
        }

        // Si cambió de puesto, validar que el nuevo puesto no tenga ya un candidato asignado
        if (asignacionExistente.PuestoElectivoId != dto.PuestoElectivoId)
        {
            var tieneAsignacionPuesto = await _unitOfWork.AsignacionesCandidatos.PuestoTieneAsignacionEnPartidoAsync(dto.PuestoElectivoId, dto.PartidoPoliticoId);
            if (tieneAsignacionPuesto)
            {
                throw new ValidacionException("Este puesto ya tiene un candidato asignado dentro de este partido.");
            }
        }

        _mapper.Map(dto, asignacionExistente);

        _repository.Update(asignacionExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    public override async Task EliminarAsync(int id)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());
        await base.EliminarAsync(id);
    }

    // Filtrar por partido
    public async Task<IEnumerable<AsignacionCandidatoPuestoDto>> ObtenerPorPartidoAsync(int partidoId)
    {
        // Se llama al método específico que incluye las relaciones (Candidato, PuestoElectivo, PartidoPolitico)
        var asignaciones = await _unitOfWork.AsignacionesCandidatos.GetByPartidoAsync(partidoId);
        return _mapper.Map<IEnumerable<AsignacionCandidatoPuestoDto>>(asignaciones);
    }

    public override async Task EliminarAsync(int id)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        await base.EliminarAsync(id);
    }
}