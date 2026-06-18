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
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        if (dto.CandidatoId <= 0 || dto.PuestoElectivoId <= 0 || dto.PartidoPoliticoId <= 0)
        {
            throw new ValidacionException("Debe seleccionar un candidato, un puesto electivo y un partido político válidos.");
        }

        var candidato = await _unitOfWork.Candidatos.GetByIdAsync(dto.CandidatoId);
        if (candidato == null || !candidato.Activo)
        {
            throw new ValidacionException("El candidato seleccionado no es válido o está inactivo.");
        }

        bool esAliado = candidato.PartidoPoliticoId != dto.PartidoPoliticoId;
        dto.EsAliado = esAliado;

        if (esAliado)
        {
            // Validar que exista una alianza vigente entre ambos partidos
            bool existeAlianza = await _unitOfWork.AlianzasPoliticas.ExisteAlianzaVigenteAsync(dto.PartidoPoliticoId, candidato.PartidoPoliticoId);
            if (!existeAlianza)
            {
                throw new ValidacionException("No existe una alianza política vigente con el partido del candidato seleccionado.");
            }

            // Validar que el candidato aliado tenga un puesto en su partido de origen
            var asignacionesOrigen = await _unitOfWork.AsignacionesCandidatos.FindAsync(a => a.CandidatoId == dto.CandidatoId && a.PartidoPoliticoId == candidato.PartidoPoliticoId);
            var asignacionOrigen = asignacionesOrigen.FirstOrDefault();

            if (asignacionOrigen == null)
            {
                throw new InvalidOperationException("Este candidato aliado no tiene un puesto asignado en su partido de origen.");
            }

            // Validar que solo aspire al mismo puesto que tiene en su partido de origen
            AsignacionCandidatoRules.ValidarCandidatoAliadoMismoPuesto(asignacionOrigen.PuestoElectivoId, dto.PuestoElectivoId);
        }

        // Validaciones propias del partido (para evitar 2 candidatos al mismo puesto o 1 candidato a 2 puestos)
        AsignacionCandidatoRules.ValidarCandidatoSinAsignacionEnPartido(await _unitOfWork.AsignacionesCandidatos.CandidatoTieneAsignacionEnPartidoAsync(dto.CandidatoId, dto.PartidoPoliticoId));
        AsignacionCandidatoRules.ValidarPuestoSinAsignacionEnPartido(await _unitOfWork.AsignacionesCandidatos.PuestoTieneAsignacionEnPartidoAsync(dto.PuestoElectivoId, dto.PartidoPoliticoId));

        var asignacion = _mapper.Map<AsignacionCandidatoPuesto>(dto);

        await _repository.AddAsync(asignacion);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AsignacionCandidatoPuestoDto>(asignacion);
    }

    // Validaciones específicas al actualizar
    public override async Task ActualizarAsync(int id, AsignacionCandidatoPuestoDto dto)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

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

    public override async Task EliminarAsync(int id)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());
        await base.EliminarAsync(id);
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