using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class CandidatoService : GenericService<Candidato, CandidatoDto>, ICandidatoService
{
    public CandidatoService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.Candidatos)
    {
    }

    public override async Task<CandidatoDto> CrearAsync(CandidatoDto dto)
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

        await _repository.AddAsync(candidato);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CandidatoDto>(candidato);
    }

    // Validaciones específicas al actualizar
    public override async Task ActualizarAsync(int id, CandidatoDto dto)
    {
        var candidatoExistente = await _repository.GetByIdAsync(id);
        if (candidatoExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(Candidato), id);
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido))
        {
            throw new ValidacionException("El nombre y el apellido no pueden estar vacíos.");
        }

        _mapper.Map(dto, candidatoExistente);

        _repository.Update(candidatoExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    // Filtrar por partido
    public async Task<IEnumerable<CandidatoDto>> ObtenerPorPartidoAsync(int partidoId)
    {
        var candidatos = await _unitOfWork.Candidatos.FindAsync(c => c.PartidoPoliticoId == partidoId);
        return _mapper.Map<IEnumerable<CandidatoDto>>(candidatos);
    }

    public async Task CambiarEstadoAsync(int id)
    {
        var candidato = await _repository.GetByIdAsync(id);
        if (candidato != null)
        {
            // Si está activo (true), pasa a inactivo (false) y viceversa.
            candidato.Activo = !candidato.Activo;

            _repository.Update(candidato);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new RegistroNoEncontradoException(nameof(Candidato), id);
        }
    }
}