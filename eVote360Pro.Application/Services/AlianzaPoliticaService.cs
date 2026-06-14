using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class AlianzaPoliticaService : IAlianzaPoliticaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AlianzaPoliticaService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlianzaPoliticaDto>> ObtenerTodasAsync()
    {
        // En un escenario real, aquí podrías necesitar un "Include" en el repositorio 
        // para traer los nombres de los partidos
        var alianzas = await _unitOfWork.AlianzasPoliticas.GetAllAsync();
        return _mapper.Map<IEnumerable<AlianzaPoliticaDto>>(alianzas);
    }

    public async Task<AlianzaPoliticaDto?> ObtenerPorIdAsync(int id)
    {
        var alianza = await _unitOfWork.AlianzasPoliticas.GetByIdAsync(id);
        return _mapper.Map<AlianzaPoliticaDto>(alianza);
    }

    public async Task<AlianzaPoliticaDto> CrearAsync(AlianzaPoliticaDto dto)
    {
        // Un partido no puede aliarse consigo mismo
        if (dto.PartidoSolicitanteId == dto.PartidoReceptorId)
        {
            throw new ValidacionException("Un partido político no puede solicitar una alianza consigo mismo.");
        }

        var alianza = _mapper.Map<AlianzaPolitica>(dto);

        // Las nuevas alianzas siempre inician como pendientes
        alianza.Estado = EstadoAlianza.Pendiente;

        await _unitOfWork.AlianzasPoliticas.AddAsync(alianza);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AlianzaPoliticaDto>(alianza);
    }

    public async Task ResponderSolicitudAsync(int id, EstadoAlianza nuevoEstado)
    {
        var alianzaExistente = await _unitOfWork.AlianzasPoliticas.GetByIdAsync(id);

        if (alianzaExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(AlianzaPolitica), id);
        }

        alianzaExistente.Estado = nuevoEstado;
        alianzaExistente.FechaRespuesta = DateTime.UtcNow; // Registramos cuándo se respondió

        _unitOfWork.AlianzasPoliticas.Update(alianzaExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var alianza = await _unitOfWork.AlianzasPoliticas.GetByIdAsync(id);

        if (alianza != null)
        {
            _unitOfWork.AlianzasPoliticas.Remove(alianza);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new RegistroNoEncontradoException(nameof(AlianzaPolitica), id);
        }
    }
}