using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

namespace eVote360Pro.Application.Services;

public class AlianzaPoliticaService : GenericService<AlianzaPolitica, AlianzaPoliticaDto>, IAlianzaPoliticaService
{
    public AlianzaPoliticaService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.AlianzasPoliticas)
    {
    }

    public override async Task<AlianzaPoliticaDto> CrearAsync(AlianzaPoliticaDto dto)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        // Un partido no puede aliarse consigo mismo
        AlianzaRules.ValidarNoEsMismoPartido(dto.PartidoSolicitanteId, dto.PartidoReceptorId);

        // Validar que no haya solicitud pendiente entre estos partidos (en cualquier dirección)
        var existePendiente = await _unitOfWork.AlianzasPoliticas
            .ExisteSolicitudPendienteAsync(dto.PartidoSolicitanteId, dto.PartidoReceptorId);
        AlianzaRules.ValidarNoExisteSolicitudPendiente(existePendiente);

        // Validar que no haya ya una alianza vigente entre ellos
        var existeVigente = await _unitOfWork.AlianzasPoliticas
            .ExisteAlianzaVigenteAsync(dto.PartidoSolicitanteId, dto.PartidoReceptorId);
        if (existeVigente)
            throw new ValidacionException("Ya existe una alianza activa entre estos partidos.");

        var alianza = _mapper.Map<AlianzaPolitica>(dto);

        // Las nuevas alianzas siempre inician como pendientes
        alianza.Estado = EstadoAlianza.Pendiente;

        await _repository.AddAsync(alianza);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AlianzaPoliticaDto>(alianza);
    }

    // filtrar por partido
    public async Task<IEnumerable<AlianzaPoliticaDto>> ObtenerPorPartidoAsync(int partidoId)
    {
        // Incluye los partidos para poder mostrar sus nombres en la vista
        var alianzas = await _unitOfWork.AlianzasPoliticas.GetPorPartidoConNombresAsync(partidoId);
        return _mapper.Map<IEnumerable<AlianzaPoliticaDto>>(alianzas);
    }

    // Aceptar o rechazar una solicitud de alianza
    public async Task ResponderSolicitudAsync(int id, EstadoAlianza nuevoEstado)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var alianzaExistente = await _repository.GetByIdAsync(id);

        if (alianzaExistente == null)
            throw new RegistroNoEncontradoException(nameof(AlianzaPolitica), id);

        alianzaExistente.Estado = nuevoEstado;
        alianzaExistente.FechaRespuesta = DateTime.UtcNow;

        _repository.Update(alianzaExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    // Cancelar una solicitud pendiente (solo el solicitante puede hacerlo)
    public async Task CancelarSolicitudAsync(int id, int partidoSolicitanteId)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var alianza = await _repository.GetByIdAsync(id);

        if (alianza == null)
            throw new RegistroNoEncontradoException(nameof(AlianzaPolitica), id);

        if (alianza.PartidoSolicitanteId != partidoSolicitanteId)
            throw new ValidacionException("Solo el partido solicitante puede cancelar esta solicitud.");

        if (alianza.Estado != EstadoAlianza.Pendiente)
            throw new ValidacionException("Solo se pueden cancelar solicitudes en estado Pendiente.");

        // Eliminación física: la solicitud desaparece limpiamente
        await _unitOfWork.AlianzasPoliticas.EliminarFisicoAsync(alianza);
        await _unitOfWork.SaveChangesAsync();
    }

    // Romper una alianza vigente (cualquiera de los dos partidos puede iniciar)
    public async Task RomperAlianzaAsync(int id, int partidoId)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var alianza = await _repository.GetByIdAsync(id);

        if (alianza == null)
            throw new RegistroNoEncontradoException(nameof(AlianzaPolitica), id);

        // Solo los partidos que forman parte de la alianza pueden romperla
        if (alianza.PartidoSolicitanteId != partidoId && alianza.PartidoReceptorId != partidoId)
            throw new ValidacionException("No tienes permiso para romper esta alianza.");

        if (alianza.Estado != EstadoAlianza.Aceptada)
            throw new ValidacionException("Solo se pueden romper alianzas en estado Aceptada.");

        // Validar que no haya candidatos aliados mezclados antes de romper
        var tieneAsignacionesAliadas = await _unitOfWork.AsignacionesCandidatos
            .ExistenAsignacionesAliadasPorAlianzaAsync(alianza.PartidoSolicitanteId, alianza.PartidoReceptorId);

        AlianzaRules.ValidarPuedeEliminarse(tieneAsignacionesAliadas);

        // Eliminación física: la alianza desaparece limpiamente
        await _unitOfWork.AlianzasPoliticas.EliminarFisicoAsync(alianza);
        await _unitOfWork.SaveChangesAsync();
    }
}
