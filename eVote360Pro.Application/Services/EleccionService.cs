using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Domain.Interfaces.Repositories;
using eVote360Pro.Domain.Rules;

namespace eVote360Pro.Application.Services;

public class EleccionService : GenericService<Eleccion, EleccionDto>, IEleccionService
{
    public EleccionService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper, unitOfWork.Elecciones) { }

    public async Task<IEnumerable<EleccionDto>> ObtenerOrdenadaPorFechaAsync()
    {
        var elecciones = await _unitOfWork.Elecciones.GetOrdenadaPorFechaDescAsync();
        return _mapper.Map<IEnumerable<EleccionDto>>(elecciones);
    }

    public async Task<IEnumerable<EleccionDto>> ObtenerPorAnioAsync(int anio)
    {
        var elecciones = await _unitOfWork.Elecciones.GetByAnioAsync(anio);
        return _mapper.Map<IEnumerable<EleccionDto>>(elecciones);
    }

    public async Task<EleccionDto?> ObtenerActivaAsync()
    {
        var eleccion = await _unitOfWork.Elecciones.GetEleccionActivaAsync();
        return eleccion == null ? null : _mapper.Map<EleccionDto>(eleccion);
    }

    public async Task<bool> ExisteEleccionActivaAsync() =>
        await _unitOfWork.Elecciones.ExisteEleccionActivaAsync();

    public async Task CrearConPuestosAsync(EleccionDto dto, List<int> puestosIds)
    {
        if (puestosIds == null || !puestosIds.Any())
            throw new InvalidOperationException("Debe seleccionar al menos un puesto electivo.");

        var eleccion = new Eleccion
        {
            Nombre = dto.Nombre.Trim(),
            FechaRealizacion = dto.FechaRealizacion,
            Estado = EstadoEleccion.Pendiente
        };

        await _unitOfWork.Elecciones.AddAsync(eleccion);
        await _unitOfWork.SaveChangesAsync();

        foreach (var puestoId in puestosIds)
        {
            await _unitOfWork.EleccionPuestos.AddAsync(new EleccionPuesto
            {
                EleccionId = eleccion.Id,
                PuestoElectivoId = puestoId
            });
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ActivarAsync(int id)
    {
        var eleccion = await ObtenerEntidadOExcepcionAsync(id);

        EleccionRules.ValidarPuedeActivarse(eleccion.Estado, await ExisteEleccionActivaAsync());
        await ValidarConfiguracionElectoralAsync(id);

        eleccion.Estado = EstadoEleccion.Activa;
        eleccion.FechaActivacion = DateTime.UtcNow;

        _unitOfWork.Elecciones.Update(eleccion);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task FinalizarAsync(int id)
    {
        var eleccion = await ObtenerEntidadOExcepcionAsync(id);
        EleccionRules.ValidarPuedeFinalizarse(eleccion.Estado);

        eleccion.Estado = EstadoEleccion.Finalizada;
        eleccion.FechaFinalizacion = DateTime.UtcNow;

        _unitOfWork.Elecciones.Update(eleccion);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ResultadoEleccionDto> ObtenerResultadosAsync(int id)
    {
        var eleccion = await ObtenerEntidadOExcepcionAsync(id);
        EleccionRules.ValidarPuedeVerResultados(eleccion.Estado);

        // Aquí invocarías tu lógica de mapeo para generar el resultado
        // ...
        return new ResultadoEleccionDto();
    }

    public async Task<IEnumerable<ResumenEleccionDto>> ObtenerResumenPorAnioAsync(int anio)
    {
        var elecciones = await _unitOfWork.Elecciones.GetByAnioAsync(anio);
        var resumenes = new List<ResumenEleccionDto>();

        foreach (var e in elecciones)
        {
            var puestos = await _unitOfWork.EleccionPuestos.GetByEleccionAsync(e.Id);
            var votantes = await _unitOfWork.ParticipacionesElectorales.ContarParticipantesPorEleccionAsync(e.Id);
            var partidosIds = new HashSet<int>();
            var candidatosIds = new HashSet<int>();

            foreach (var ep in puestos)
            {
                var asignaciones = await _unitOfWork.AsignacionesCandidatos.FindAsync(a => a.PuestoElectivoId == ep.PuestoElectivoId);
                foreach (var a in asignaciones)
                {
                    partidosIds.Add(a.PartidoPoliticoId);
                    candidatosIds.Add(a.CandidatoId);
                }
            }

            resumenes.Add(new ResumenEleccionDto
            {
                Id = e.Id,
                NombreEleccion = e.Nombre,
                Estado = e.Estado.ToString(),
                TotalPartidos = partidosIds.Count,
                TotalCandidatos = candidatosIds.Count,
                TotalVotantes = votantes
            });
        }
        return resumenes;
    }

    private async Task<Eleccion> ObtenerEntidadOExcepcionAsync(int id) =>
        await _unitOfWork.Elecciones.GetByIdAsync(id) ?? throw new RegistroNoEncontradoException(nameof(Eleccion), id);

    private async Task ValidarConfiguracionElectoralAsync(int eleccionId)
    {
        var puestos = await _unitOfWork.EleccionPuestos.GetByEleccionAsync(eleccionId);
        if (!puestos.Any()) throw new InvalidOperationException("La elección no tiene puestos configurados.");

        foreach (var ep in puestos)
        {
            var asignaciones = await _unitOfWork.AsignacionesCandidatos.FindAsync(a => a.PuestoElectivoId == ep.PuestoElectivoId);
            if (!asignaciones.Any())
                throw new InvalidOperationException($"El puesto '{ep.PuestoElectivo?.Nombre}' no tiene candidatos asignados.");
        }
    }
}