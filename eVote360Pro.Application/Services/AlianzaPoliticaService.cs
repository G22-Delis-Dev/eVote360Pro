using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class AlianzaPoliticaService : GenericService<AlianzaPolitica, AlianzaPoliticaDto>, IAlianzaPoliticaService
{
    public AlianzaPoliticaService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.AlianzasPoliticas)
    {
    }

    public override async Task<AlianzaPoliticaDto> CrearAsync(AlianzaPoliticaDto dto)
    {
        // Un partido no puede aliarse consigo mismo
        if (dto.PartidoSolicitanteId == dto.PartidoReceptorId)
        {
            throw new ValidacionException("Un partido político no puede solicitar una alianza consigo mismo.");
        }

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
}