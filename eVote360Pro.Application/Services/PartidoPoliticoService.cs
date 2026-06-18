using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

namespace eVote360Pro.Application.Services;

public class PartidoPoliticoService : GenericService<PartidoPolitico, PartidoPoliticoDto>, IPartidoPoliticoService
{
    public PartidoPoliticoService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.PartidosPoliticos) { }

    public async Task<IEnumerable<PartidoPoliticoDto>> ObtenerActivosAsync()
    {
        var partidos = await _unitOfWork.PartidosPoliticos.GetAllAsync();
        return _mapper.Map<IEnumerable<PartidoPoliticoDto>>(partidos.Where(p => p.Activo));
    }

    public async Task CrearAsync(PartidoPoliticoDto dto, string rutaLogo)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        PartidoPoliticoRules.ValidarSiglasUnicas(
            await _unitOfWork.PartidosPoliticos.ExisteSiglasAsync(dto.Siglas));

        var partido = _mapper.Map<PartidoPolitico>(dto);
        partido.LogoRuta = rutaLogo;
        partido.Siglas = dto.Siglas.ToUpper();
        partido.Activo = true;

        await _unitOfWork.PartidosPoliticos.AddAsync(partido);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task EditarAsync(PartidoPoliticoDto dto, string? rutaLogo)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var partido = await _unitOfWork.PartidosPoliticos.GetByIdAsync(dto.Id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(PartidoPolitico), dto.Id);

        var participoEnEleccion = await _unitOfWork.PartidosPoliticos.ParticipoEnEleccionAsync(dto.Id);

        if (participoEnEleccion)
        {
            PartidoPoliticoRules.ValidarCamposCriticosNoModificables(
                partido.Nombre != dto.Nombre ||
                partido.Siglas != dto.Siglas ||
                rutaLogo != null);
        }

        PartidoPoliticoRules.ValidarSiglasUnicas(
            await _unitOfWork.PartidosPoliticos.ExisteSiglasAsync(dto.Siglas, dto.Id));

        _mapper.Map(dto, partido);
        partido.Siglas = dto.Siglas.ToUpper();
        if (rutaLogo != null) partido.LogoRuta = rutaLogo;
        partido.FechaModificacion = DateTime.UtcNow;

        _unitOfWork.PartidosPoliticos.Update(partido);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CambiarEstadoAsync(int id)
    {
        var partido = await _unitOfWork.PartidosPoliticos.GetByIdAsync(id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(PartidoPolitico), id);

        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        if (partido.Activo)
        {
            PartidoPoliticoRules.ValidarPuedeDesactivarse(
                await _unitOfWork.PartidosPoliticos.TieneCandidatosActivosAsync(id),
                await _unitOfWork.PartidosPoliticos.TieneDirigenteAsignadoAsync(id));
        }

        partido.Activo = !partido.Activo;
        partido.FechaModificacion = DateTime.UtcNow;

        _unitOfWork.PartidosPoliticos.Update(partido);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ParticipoEnEleccionAsync(int id)
    {
        return await _unitOfWork.PartidosPoliticos.ParticipoEnEleccionAsync(id);
    }
}