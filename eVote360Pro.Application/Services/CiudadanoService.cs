using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

namespace eVote360Pro.Application.Services;

public class CiudadanoService : GenericService<Ciudadano, CiudadanoDto>, ICiudadanoService
{
    public CiudadanoService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.Ciudadanos) { }

    public async Task<IEnumerable<CiudadanoDto>> ObtenerListaAsync(string? filtro = null)
    {
        var ciudadanos = string.IsNullOrWhiteSpace(filtro)
            ? await _unitOfWork.Ciudadanos.GetAllAsync()
            : await _unitOfWork.Ciudadanos.FindAsync(c =>
                c.Nombre.Contains(filtro) ||
                c.Apellido.Contains(filtro) ||
                c.NumeroDocumento.Contains(filtro));

        return _mapper.Map<IEnumerable<CiudadanoDto>>(ciudadanos);
    }

    public override async Task<CiudadanoDto> CrearAsync(CiudadanoDto dto)
    {
        CiudadanoRules.ValidarDocumentoUnico(
            await _unitOfWork.Ciudadanos.ExisteNumeroDocumentoAsync(dto.NumeroDocumento));

        CiudadanoRules.ValidarCorreoUnico(
            await _unitOfWork.Ciudadanos.ExisteCorreoElectronicoAsync(dto.CorreoElectronico));


        dto.Activo = true;
        return await base.CrearAsync(dto);
    }

    public async Task EditarAsync(CiudadanoDto dto)
    {
        var ciudadano = await _unitOfWork.Ciudadanos.GetByIdAsync(dto.Id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(Ciudadano), dto.Id);

        var participoEnEleccion = await _unitOfWork.Ciudadanos.ParticipóEnEleccionAsync(dto.Id);

        CiudadanoRules.ValidarDocumentoNoModificable(
            participoEnEleccion && ciudadano.NumeroDocumento != dto.NumeroDocumento);

        CiudadanoRules.ValidarDocumentoUnico(
            await _unitOfWork.Ciudadanos.ExisteNumeroDocumentoAsync(dto.NumeroDocumento, dto.Id));

        CiudadanoRules.ValidarCorreoUnico(
            await _unitOfWork.Ciudadanos.ExisteCorreoElectronicoAsync(dto.CorreoElectronico, dto.Id));

        _mapper.Map(dto, ciudadano);
        ciudadano.FechaModificacion = DateTime.UtcNow;

        _unitOfWork.Ciudadanos.Update(ciudadano);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CambiarEstadoAsync(int id)
    {
        var ciudadano = await _unitOfWork.Ciudadanos.GetByIdAsync(id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(Ciudadano), id);

        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        ciudadano.Activo = !ciudadano.Activo;
        ciudadano.FechaModificacion = DateTime.UtcNow;

        _unitOfWork.Ciudadanos.Update(ciudadano);
        await _unitOfWork.SaveChangesAsync();
    }
}