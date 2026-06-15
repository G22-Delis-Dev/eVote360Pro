using AutoMapper;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public abstract class GenericService<TEntity, TDto> : IGenericService<TDto>
    where TEntity : BaseEntity
    where TDto : class
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;
    protected readonly IRepository<TEntity> _repository;

    protected GenericService(IUnitOfWork unitOfWork, IMapper mapper, IRepository<TEntity> repository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _repository = repository;
    }

    public virtual async Task<IEnumerable<TDto>> ObtenerTodosAsync()
    {
        var entidades = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TDto>>(entidades);
    }

    public virtual async Task<TDto?> ObtenerPorIdAsync(int id)
    {
        var entidad = await _repository.GetByIdAsync(id);
        return _mapper.Map<TDto>(entidad);
    }

    public virtual async Task<TDto> CrearAsync(TDto dto)
    {
        var entidad = _mapper.Map<TEntity>(dto);

        await _repository.AddAsync(entidad);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TDto>(entidad);
    }

    public virtual async Task ActualizarAsync(int id, TDto dto)
    {
        var entidadExistente = await _repository.GetByIdAsync(id);
        if (entidadExistente == null)
        {
            throw new RegistroNoEncontradoException(typeof(TEntity).Name, id);
        }

        _mapper.Map(dto, entidadExistente);

        _repository.Update(entidadExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    public virtual async Task EliminarAsync(int id)
    {
        var entidad = await _repository.GetByIdAsync(id);
        if (entidad == null)
        {
            throw new RegistroNoEncontradoException(typeof(TEntity).Name, id);
        }

        _repository.Remove(entidad);
        await _unitOfWork.SaveChangesAsync();
    }
}
