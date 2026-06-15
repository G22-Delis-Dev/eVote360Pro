using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

namespace eVote360Pro.Application.Services;

public class AsignacionDirigenteService : GenericService<AsignacionDirigente, AsignacionDirigenteDto>, IAsignacionDirigenteService
{
    public AsignacionDirigenteService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.AsignacionesDirigentes) { }

    public async Task<IEnumerable<AsignacionDirigenteDto>> ObtenerListaAsync()
    {
        var asignaciones = await _unitOfWork.AsignacionesDirigentes.GetAllAsync();
        return _mapper.Map<IEnumerable<AsignacionDirigenteDto>>(asignaciones);
    }

    public async Task<IEnumerable<object>> ObtenerDirigentesDisponiblesAsync()
    {
        var dirigentes = await _unitOfWork.Usuarios.GetDirigentesDisponiblesParaAsignacionAsync();
        return dirigentes.Select(u => new { Value = u.Id, Text = $"{u.Nombre} {u.Apellido}" });
    }

    public async Task<IEnumerable<object>> ObtenerPartidosDisponiblesAsync()
    {
        var partidos = await _unitOfWork.PartidosPoliticos.GetActivosDisponiblesParaAsignacionAsync();
        return partidos.Select(p => new { Value = p.Id, Text = $"{p.Nombre} ({p.Siglas})" });
    }

    public override async Task<AsignacionDirigenteDto> CrearAsync(AsignacionDirigenteDto dto)
    {
        // Validación de regla de negocio antes de crear
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        if (await _unitOfWork.AsignacionesDirigentes.DirigenteTienePartidoAsync(dto.UsuarioId))
            throw new InvalidOperationException("Este dirigente ya tiene un partido asignado.");

        if (await _unitOfWork.AsignacionesDirigentes.PartidoTieneDirigenteAsync(dto.PartidoPoliticoId))
            throw new InvalidOperationException("Este partido ya tiene un dirigente asignado.");

        return await base.CrearAsync(dto);
    }
}