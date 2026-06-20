using AutoMapper;
using BCrypt.Net;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

namespace eVote360Pro.Application.Services;

public class UsuarioService : GenericService<Usuario, UsuarioDto>, IUsuarioService
{
    public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.Usuarios) { }

    public async Task<IEnumerable<UsuarioDto>> ObtenerListaAsync()
    {
        var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
        return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);
    }

    public async Task CrearAsync(UsuarioDto dto, string password)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        UsuarioRules.ValidarNombreUsuarioUnico(
            await _unitOfWork.Usuarios.ExisteNombreUsuarioAsync(dto.NombreUsuario));

        // Corregido: dto.Correo -> dto.CorreoElectronico
        UsuarioRules.ValidarCorreoUnico(
            await _unitOfWork.Usuarios.ExisteCorreoElectronicoAsync(dto.CorreoElectronico));

        var usuario = _mapper.Map<Usuario>(dto);
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        usuario.Activo = true;

        await _unitOfWork.Usuarios.AddAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task EditarAsync(UsuarioDto dto, string? nuevaPassword = null)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(dto.Id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(Usuario), dto.Id);

        UsuarioRules.ValidarNombreUsuarioUnico(
            await _unitOfWork.Usuarios.ExisteNombreUsuarioAsync(dto.NombreUsuario, dto.Id));

        // Corregido: dto.Correo -> dto.CorreoElectronico
        UsuarioRules.ValidarCorreoUnico(
            await _unitOfWork.Usuarios.ExisteCorreoElectronicoAsync(dto.CorreoElectronico, dto.Id));

        if (usuario.Rol == RolUsuario.DirigentePolitico && dto.Rol != usuario.Rol)
        {
            var tieneDirigente = await _unitOfWork.AsignacionesDirigentes.DirigenteTienePartidoAsync(usuario.Id);
            UsuarioRules.ValidarRolCambiable(tieneDirigente);
        }

        _mapper.Map(dto, usuario);
        usuario.FechaModificacion = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(nuevaPassword))
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);

        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ToggleActivoAsync(int id, int usuarioActualId)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id)
            ?? throw new Domain.Exceptions.RegistroNoEncontradoException(nameof(Usuario), id);

        UsuarioRules.ValidarNoEsMismoUsuario(usuarioActualId, id);

        if (usuario.Rol == RolUsuario.Administrador && usuario.Activo)
        {
            var totalAdmins = await _unitOfWork.Usuarios.ContarAdministradoresActivosAsync();
            UsuarioRules.ValidarUnicoAdministrador(totalAdmins);
        }

        usuario.Activo = !usuario.Activo;
        usuario.FechaModificacion = DateTime.UtcNow;

        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<UsuarioDto?> ValidarCredencialesAsync(string nombreUsuario, string password)
    {
        var usuario = await _unitOfWork.Usuarios.GetByNombreUsuarioAsync(nombreUsuario);

        if (usuario == null || !usuario.Activo)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            return null;

        return _mapper.Map<UsuarioDto>(usuario);
    }
}