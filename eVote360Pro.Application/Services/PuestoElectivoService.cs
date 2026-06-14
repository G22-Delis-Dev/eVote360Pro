using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class PuestoElectivoService : IPuestoElectivoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PuestoElectivoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PuestoElectivoDto>> ObtenerTodosAsync()
    {
        var puestos = await _unitOfWork.PuestosElectivos.GetAllAsync();
        return _mapper.Map<IEnumerable<PuestoElectivoDto>>(puestos);
    }
}