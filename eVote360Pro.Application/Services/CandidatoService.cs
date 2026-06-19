using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

namespace eVote360Pro.Application.Services;

public class CandidatoService : GenericService<Candidato, CandidatoDto>, ICandidatoService
{
    public CandidatoService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.Candidatos)
    {
    }

    public override async Task<CandidatoDto> CrearAsync(CandidatoDto dto)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

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
        EleccionRules.ValidarNoExisteEleccionActiva(
            await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var candidatoExistente = await _repository.GetByIdAsync(id);
        if (candidatoExistente == null)
        {
            throw new RegistroNoEncontradoException(nameof(Candidato), id);
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido))
        {
            throw new ValidacionException("El nombre y el apellido no pueden estar vacíos.");
        }

        bool participo = await _unitOfWork.Candidatos.ParticipoEnEleccionAsync(id);
        if (participo)
        {
            // Validar si los campos críticos han cambiado
            bool cambiaronCriticos = candidatoExistente.Nombre != dto.Nombre || 
                                     candidatoExistente.Apellido != dto.Apellido || 
                                     (dto.FotoUrl != null && candidatoExistente.FotoRuta != dto.FotoUrl);
            
            if (cambiaronCriticos)
            {
                CandidatoRules.ValidarCamposCriticosNoModificables(participo);
            }
        }

        _mapper.Map(dto, candidatoExistente);

        _repository.Update(candidatoExistente);
        await _unitOfWork.SaveChangesAsync();
    }

    // Filtrar por partido
    public async Task<IEnumerable<CandidatoDto>> ObtenerPorPartidoAsync(int partidoId)
    {
        var candidatos = await _unitOfWork.Candidatos.GetByPartidoConPuestosAsync(partidoId);
        return _mapper.Map<IEnumerable<CandidatoDto>>(candidatos);
    }

    // Obtener candidatos activos de los partidos aliados del partido dado
    public async Task<IEnumerable<CandidatoDto>> ObtenerAliadosPorPartidoAsync(int partidoId)
    {
        // Buscar alianzas vigentes (aceptadas) del partido
        var alianzas = await _unitOfWork.AlianzasPoliticas.GetAlianzasVigentesAsync(partidoId);

        // Extraer los IDs de los partidos aliados (el otro lado de cada alianza)
        var idsPartidosAliados = alianzas.Select(a =>
            a.PartidoSolicitanteId == partidoId ? a.PartidoReceptorId : a.PartidoSolicitanteId
        ).Distinct().ToList();

        if (!idsPartidosAliados.Any())
            return [];

        var candidatos = await _unitOfWork.Candidatos.GetActivosByPartidosAsync(idsPartidosAliados);
        return _mapper.Map<IEnumerable<CandidatoDto>>(candidatos);
    }

    public async Task CambiarEstadoAsync(int id)
    {
        EleccionRules.ValidarNoExisteEleccionActiva(await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

        var candidato = await _repository.GetByIdAsync(id);
        if (candidato == null)
            throw new RegistroNoEncontradoException(nameof(Candidato), id);

        // Solo validar el bloqueo si se está intentando desactivar (pasar de activo a inactivo)
        if (candidato.Activo)
        {
            EleccionRules.ValidarNoExisteEleccionActiva(
                await _unitOfWork.Elecciones.ExisteEleccionActivaAsync());

            var estaAsignado = await _unitOfWork.Candidatos.EstaAsignadoAPuestoAsync(id);
            CandidatoRules.ValidarPuedeDesactivarse(estaAsignado);
        }

        candidato.Activo = !candidato.Activo;
        _repository.Update(candidato);
        await _unitOfWork.SaveChangesAsync();
    }
}