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

        // Total de ciudadanos que participaron en esta elección
        var totalVotantes = await _unitOfWork.ParticipacionesElectorales
            .ContarParticipantesPorEleccionAsync(id);

        // Puestos configurados para esta elección
        var puestosEleccion = await _unitOfWork.EleccionPuestos.GetByEleccionAsync(id);

        var resultadosPorPuesto = new List<ResultadoPuestoDto>();

        foreach (var ep in puestosEleccion)
        {
            // Candidatos asignados a este puesto (de cualquier partido, incluidos aliados)
            var asignacionesPuesto = await _unitOfWork.AsignacionesCandidatos
                .FindAsync(a => a.PuestoElectivoId == ep.PuestoElectivoId && a.Activo);

            // Votos emitidos para este puesto en esta elección
            var votosPuesto = await _unitOfWork.Votos
                .GetByEleccionYPuestoAsync(id, ep.PuestoElectivoId);

            int totalVotosPuesto = votosPuesto.Count();

            var resultadosCandidatos = new List<ResultadoCandidatoDto>();

            foreach (var asignacion in asignacionesPuesto)
            {
                // Traer datos del candidato con su partido
                var candidato = await _unitOfWork.Candidatos.GetByIdAsync(asignacion.CandidatoId);
                var partido = await _unitOfWork.PartidosPoliticos.GetByIdAsync(asignacion.PartidoPoliticoId);

                int votosObtenidos = await _unitOfWork.Votos
                    .ContarVotosPorCandidatoAsync(id, ep.PuestoElectivoId, asignacion.CandidatoId);

                double porcentaje = totalVotosPuesto > 0
                    ? Math.Round((double)votosObtenidos / totalVotosPuesto * 100, 2)
                    : 0;

                resultadosCandidatos.Add(new ResultadoCandidatoDto
                {
                    CandidatoId = asignacion.CandidatoId,
                    NombreCandidato = candidato != null
                        ? $"{candidato.Nombre} {candidato.Apellido}"
                        : "Candidato desconocido",
                    NombrePartido = partido?.Nombre ?? "Partido desconocido",
                    LogoPartido = partido?.LogoRuta ?? string.Empty,
                    TotalVotos = votosObtenidos,
                    Porcentaje = porcentaje,
                    EsGanador = false, // se calcula después
                    EsEmpate = false
                });
            }

            // Determinar ganador(es)
            if (resultadosCandidatos.Any())
            {
                int maxVotos = resultadosCandidatos.Max(c => c.TotalVotos);
                var ganadores = resultadosCandidatos.Where(c => c.TotalVotos == maxVotos).ToList();

                if (ganadores.Count == 1)
                {
                    ganadores[0].EsGanador = true;
                }
                else
                {
                    // empate
                    foreach (var g in ganadores)
                    {
                        g.EsEmpate = true;
                        g.EsGanador = true;
                    }
                }
            }

            resultadosPorPuesto.Add(new ResultadoPuestoDto
            {
                PuestoId = ep.PuestoElectivoId,
                NombrePuesto = ep.PuestoElectivo?.Nombre ?? "Puesto desconocido",
                TotalVotos = totalVotosPuesto,
                Candidatos = resultadosCandidatos.OrderByDescending(c => c.TotalVotos).ToList()
            });
        }

        return new ResultadoEleccionDto
        {
            EleccionId = eleccion.Id,
            NombreEleccion = eleccion.Nombre,
            TotalVotantes = totalVotantes,
            Puestos = resultadosPorPuesto
        };
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