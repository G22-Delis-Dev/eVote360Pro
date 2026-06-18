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
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        // Un partido no puede aliarse consigo mismo
        if (dto.PartidoSolicitanteId == dto.PartidoReceptorId)
        {
            throw new ValidacionException("Un partido político no puede solicitar una alianza consigo mismo.");
        }

        AlianzaRules.ValidarNoExisteSolicitudPendiente(await _unitOfWork.AlianzasPoliticas.ExisteSolicitudPendienteAsync(dto.PartidoSolicitanteId, dto.PartidoReceptorId));

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
        // Solo devuelve las alianzas donde el partido es solicitante o receptor
        var alianzas = await _unitOfWork.AlianzasPoliticas
            .FindAsync(a => a.PartidoSolicitanteId == partidoId || a.PartidoReceptorId == partidoId);
        return _mapper.Map<IEnumerable<AlianzaPoliticaDto>>(alianzas);
    }

    // Aceptar o rechazar una solicitud de alianza
    public async Task ResponderSolicitudAsync(int id, EstadoAlianza nuevoEstado)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var alianzaExistente = await _repository.GetByIdAsync(id);

        if (alianzaExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(AlianzaPolitica), id);
        }

        alianzaExistente.Estado = nuevoEstado;
        alianzaExistente.FechaRespuesta = DateTime.UtcNow; // Registramos cuándo se respondió

        _repository.Update(alianzaExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<AlianzaPoliticaDto>> ObtenerAlianzasVigentesAsync(int partidoId)
    {
        var alianzas = await _unitOfWork.AlianzasPoliticas.GetAlianzasVigentesAsync(partidoId);
        return _mapper.Map<IEnumerable<AlianzaPoliticaDto>>(alianzas);
    }

    public override async Task EliminarAsync(int id)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var alianza = await _repository.GetByIdAsync(id) ?? throw new RegistroNoEncontradoException(nameof(AlianzaPolitica), id);

        if (alianza.Estado == EstadoAlianza.Aceptada)
        {
            bool tieneAliados = await _unitOfWork.AsignacionesCandidatos.ExistenAsignacionesAliadasPorAlianzaAsync(alianza.PartidoSolicitanteId, alianza.PartidoReceptorId);
            AlianzaRules.ValidarPuedeEliminarse(tieneAliados);
        }

        _repository.Remove(alianza);
        await _unitOfWork.SaveChangesAsync();
    }
}