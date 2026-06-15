using AutoMapper;
using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Enums;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Domain.Interfaces.Repositories;

namespace eVote360Pro.Application.Services;

public class VotacionService : IVotacionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;

    public VotacionService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IEmailTemplateService templateService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _emailService = emailService;
        _templateService = templateService;
    }

    public async Task<EleccionDto?> ObtenerEleccionActivaAsync()
    {
        var elecciones = await _unitOfWork.Elecciones.FindAsync(e => e.Estado == EstadoEleccion.Activa);
        var eleccionActiva = elecciones.FirstOrDefault();

        return eleccionActiva == null ? null : _mapper.Map<EleccionDto>(eleccionActiva);
    }

    public async Task<CiudadanoDto> ValidarCiudadanoParaVotarAsync(string cedula, int eleccionId)
    {
        var ciudadanos = await _unitOfWork.Ciudadanos.FindAsync(c => c.NumeroDocumento == cedula);
        var ciudadano = ciudadanos.FirstOrDefault();

        // Aquí sí usamos Activo porque la entidad Ciudadano lo tiene explícitamente
        if (ciudadano == null || !ciudadano.Activo)
        {
            throw new ValidacionException("La cédula ingresada no pertenece a un padrón electoral válido o está inactiva.");
        }

        var participacion = await _unitOfWork.ParticipacionesElectorales
            .FindAsync(p => p.CiudadanoId == ciudadano.Id && p.EleccionId == eleccionId);

        if (participacion.Any())
        {
            throw new ValidacionException("Este ciudadano ya ha emitido su voto en esta elección. Evitando fraude.");
        }

        return _mapper.Map<CiudadanoDto>(ciudadano);
    }

    public async Task GenerarYEnviarCodigoAsync(int ciudadanoId, int eleccionId)
    {
        // 1. Invalidar códigos anteriores que no se hayan utilizado
        var codigosViejos = await _unitOfWork.CodigosVerificacion
            .FindAsync(c => c.CiudadanoId == ciudadanoId && c.EleccionId == eleccionId && !c.Utilizado);

        foreach (var viejo in codigosViejos)
        {
            // Solo actualizamos Utilizado, ya que BaseEntity no tiene Activo
            viejo.Utilizado = true;
            _unitOfWork.CodigosVerificacion.Update(viejo);
        }

        // 2. Generar un código nuevo
        string nuevoCodigo = new Random().Next(100000, 1000000).ToString();

        var codigoEntidad = new CodigoVerificacion
        {
            CiudadanoId = ciudadanoId,
            EleccionId = eleccionId,
            Codigo = nuevoCodigo,
            FechaGeneracion = DateTime.UtcNow,
            FechaExpiracion = DateTime.UtcNow.AddMinutes(5),
            Utilizado = false
        };

        await _unitOfWork.CodigosVerificacion.AddAsync(codigoEntidad);
        await _unitOfWork.SaveChangesAsync();

        // 3. SIMULACIÓN DE CORREO
        Console.WriteLine("\n=======================================================");
        Console.WriteLine($"[SIMULACIÓN SMTP] ENVIANDO CORREO AL CIUDADANO...");
        Console.WriteLine($"CÓDIGO SECRETO: {nuevoCodigo}");
        Console.WriteLine("=======================================================\n");
    }

    public async Task<bool> ValidarCodigoVerificacionAsync(int ciudadanoId, int eleccionId, string codigo)
    {
        var registro = await _unitOfWork.CodigosVerificacion
            .FindAsync(c => c.CiudadanoId == ciudadanoId
                     && c.EleccionId == eleccionId
                     && c.Codigo == codigo
                     && !c.Utilizado);

        var codigoValido = registro.FirstOrDefault();

        if (codigoValido == null)
            throw new ValidacionException("El código es incorrecto o ya fue utilizado.");

        if (codigoValido.FechaExpiracion < DateTime.UtcNow)
            throw new ValidacionException("El código ha expirado. Por favor solicite uno nuevo.");

        return true;
    }

    public async Task<IEnumerable<PuestoBoletaDto>> ObtenerBoletaElectoralAsync(int eleccionId)
    {
        var eleccionPuestos = await _unitOfWork.EleccionPuestos.FindAsync(ep => ep.EleccionId == eleccionId);
        var boleta = new List<PuestoBoletaDto>();

        foreach (var ep in eleccionPuestos)
        {
            var puesto = await _unitOfWork.PuestosElectivos.GetByIdAsync(ep.PuestoElectivoId);
            if (puesto == null) continue;

            // Buscamos las asignaciones de este puesto
            var asignaciones = await _unitOfWork.AsignacionesCandidatos
                .FindAsync(a => a.PuestoElectivoId == puesto.Id);

            var candidatosDto = new List<CandidatoBoletaDto>();
            foreach (var a in asignaciones)
            {
                var candidato = await _unitOfWork.Candidatos.GetByIdAsync(a.CandidatoId);
                var partido = await _unitOfWork.PartidosPoliticos.GetByIdAsync(a.PartidoPoliticoId);

                if (candidato != null && partido != null)
                {
                    candidatosDto.Add(new CandidatoBoletaDto
                    {
                        CandidatoId = candidato.Id,
                        NombreCompleto = $"{candidato.Nombre} {candidato.Apellido}",
                        FotoUrl = candidato.FotoRuta ?? "",
                        PartidoPoliticoId = partido.Id,
                        PartidoNombre = partido.Nombre,
                        LogoPartido = partido.LogoRuta ?? "" 
                    });
                }
            }

            boleta.Add(new PuestoBoletaDto
            {
                PuestoId = puesto.Id,
                PuestoNombre = puesto.Nombre,
                Candidatos = candidatosDto
            });
        }

        return boleta;
    }

    public async Task ProcesarVotacionAsync(int ciudadanoId, int eleccionId, IEnumerable<VotoDto> votos)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Quemar el código
            var codigos = await _unitOfWork.CodigosVerificacion
                .FindAsync(c => c.CiudadanoId == ciudadanoId && c.EleccionId == eleccionId && !c.Utilizado);

            var codigoActual = codigos.FirstOrDefault();
            if (codigoActual != null)
            {
                codigoActual.Utilizado = true;
                _unitOfWork.CodigosVerificacion.Update(codigoActual);
            }

            // 2. Registrar Participación
            var participacion = new ParticipacionElectoral
            {
                CiudadanoId = ciudadanoId,
                EleccionId = eleccionId,
                FechaVoto = DateTime.UtcNow
            };
            await _unitOfWork.ParticipacionesElectorales.AddAsync(participacion);

            // 3. Registrar Votos Anónimos
            foreach (var voto in votos)
            {
                var nuevoVoto = new Voto
                {
                    EleccionId = eleccionId,
                    PuestoElectivoId = voto.PuestoElectivoId,
                    CandidatoId = voto.CandidatoId,
                    PartidoPoliticoId = voto.PartidoPoliticoId
                };
                await _unitOfWork.Votos.AddAsync(nuevoVoto);
            }

            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new Exception("Ocurrió un error crítico al procesar la votación. Su voto no fue registrado.", ex);
        }
    }

    public async Task EnviarNotificacionVotoAsync(string email, string nombre, ResumenVotacionDto resumen)
    {
        // 1. Generar HTML usando el servicio de plantillas
        var html = _templateService.GenerarResumenVotacionHtml(nombre, resumen);

        // 2. Enviar usando el servicio de correo
        await _emailService.EnviarAsync(email, "Resumen de Voto", html);
    }
}