using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class CandidatoService : ICandidatoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CandidatoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CandidatoDto>> ObtenerTodosAsync()
    {
        // Traemos todas las entidades y las mapea a DTO
        var candidatos = await _unitOfWork.Candidatos.GetAllAsync();
        return _mapper.Map<IEnumerable<CandidatoDto>>(candidatos);
    }

    public async Task<CandidatoDto?> ObtenerPorIdAsync(int id)
    {
        var candidato = await _unitOfWork.Candidatos.GetByIdAsync(id);
        return _mapper.Map<CandidatoDto>(candidato);
    }

    public async Task<IEnumerable<CandidatoDto>> ObtenerPorPartidoAsync(int partidoId)
    {
        // Usamos el repositorio para filtrar directamente en la bd
        var candidatos = await _unitOfWork.Candidatos.FindAsync(c => c.PartidoPoliticoId == partidoId);
        return _mapper.Map<IEnumerable<CandidatoDto>>(candidatos);
    }

    public async Task<CandidatoDto> CrearAsync(CandidatoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido))
        {
            throw new ValidacionException("El nombre y el apellido del candidato son obligatorios.");
        }

        if (dto.PartidoPoliticoId <= 0)
        {
            throw new ValidacionException("Debe asignar un partido político válido al candidato.");
        }

        var candidato = _mapper.Map<Candidato>(dto);

        // Por defecto, cuando creamos un candidato nuevo, nace activo
        candidato.Activo = true;

        await _unitOfWork.Candidatos.AddAsync(candidato);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CandidatoDto>(candidato);
    }

    public async Task ActualizarAsync(int id, CandidatoDto dto)
    {
        var candidatoExistente = await _unitOfWork.Candidatos.GetByIdAsync(id);
        if (candidatoExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(Candidato), id);
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido))
        {
            throw new ValidacionException("El nombre y el apellido no pueden estar vacíos.");
        }

        // Esto toma los valores del DTO y sobrescribe las propiedades de la entidad existente
        _mapper.Map(dto, candidatoExistente);

        _unitOfWork.Candidatos.Update(candidatoExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var candidato = await _unitOfWork.Candidatos.GetByIdAsync(id);
        if (candidato != null)
        {
            _unitOfWork.Candidatos.Remove(candidato);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new RegistroNoEncontradoException(nameof(Candidato), id);
        }
    }

    public async Task CambiarEstadoAsync(int id)
    {
        var candidato = await _unitOfWork.Candidatos.GetByIdAsync(id);
        if (candidato != null)
        {
            // Si está activo (true), pasa a inactivo (false) y viceversa.
            candidato.Activo = !candidato.Activo;

            _unitOfWork.Candidatos.Update(candidato);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new RegistroNoEncontradoException(nameof(Candidato), id);
        }
    }
}