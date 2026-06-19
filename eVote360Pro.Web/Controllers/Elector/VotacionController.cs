using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Votacion;
using eVote360Pro.Domain.Exceptions;
using eVote360Pro.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers.Elector
{
    public class VotacionController : Controller
    {
        private readonly IVotacionService _votacionService;

        private const string SesionVotacionKey = "VotacionSesion";
        private static readonly string[] FormatosPermitidos = [".jpg", ".jpeg", ".png"];

        public VotacionController(IVotacionService votacionService)
        {
            _votacionService = votacionService;
        }

        // ─── PANTALLA DE BIENVENIDA ───────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpContext.Session.Remove(SesionVotacionKey);

            var eleccionActiva = await _votacionService.ObtenerEleccionActivaAsync();
            if (eleccionActiva == null)
                return View("SinElecciones");

            var vm = new InicioVotacionViewModel
            {
                EleccionId       = eleccionActiva.Id,
                EleccionNombre   = eleccionActiva.Nombre,
                FechaRealizacion = eleccionActiva.FechaRealizacion
            };

            return View(vm);
        }

        // ─── PASO 1: VALIDACIÓN DE CÉDULA ────────────────────────────────────────

        [HttpGet]
        public IActionResult ValidarIdentidad()
        {
            ViewData["PasoActivo"] = 1;
            return View(new ValidacionIdentidadViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarIdentidad(ValidacionIdentidadViewModel vm)
        {
            ViewData["PasoActivo"] = 1;
            if (!ModelState.IsValid) return View(vm);

            vm.NumeroDocumento = vm.NumeroDocumento.Replace("-", "").Replace(" ", "").Trim();

            var eleccion = await _votacionService.ObtenerEleccionActivaAsync();
            if (eleccion == null) return RedirectToAction(nameof(Index));

            try
            {
                var ciudadano = await _votacionService.ValidarCiudadanoParaVotarAsync(vm.Cedula, eleccion.Id);

                var sesion = new VotacionSesionData
                {
                    CiudadanoId       = ciudadano.Id,
                    EleccionId        = eleccion.Id,
                    CedulaIngresada   = vm.Cedula,
                    CorreoElectronico = ciudadano.CorreoElectronico,
                    PasoActual        = PasoVotacion.IdentidadValidada
                };
                HttpContext.Session.Set(SesionVotacionKey, sesion);

                return RedirectToAction(nameof(ValidarOcr));
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // ─── PASO 2: VALIDACIÓN OCR ───────────────────────────────────────────────

        [HttpGet]
        public IActionResult ValidarOcr()
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.IdentidadValidada)
                return RedirectToAction(nameof(ValidarIdentidad));

            ViewData["PasoActivo"] = 2;
            var vm = new ValidacionOcrViewModel
            {
                CiudadanoId     = sesion.CiudadanoId,
                EleccionId      = sesion.EleccionId,
                CedulaIngresada = sesion.CedulaIngresada
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarOcr(ValidacionOcrViewModel vm)
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.IdentidadValidada)
                return RedirectToAction(nameof(ValidarIdentidad));

            ViewData["PasoActivo"] = 2;
            vm.CiudadanoId     = sesion.CiudadanoId;
            vm.EleccionId      = sesion.EleccionId;
            vm.CedulaIngresada = sesion.CedulaIngresada;

            if (vm.ImagenCedula == null || vm.ImagenCedula.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe subir una imagen de su cédula para validar su identidad.");
                return View(vm);
            }

            var extension = Path.GetExtension(vm.ImagenCedula.FileName).ToLowerInvariant();
            if (!FormatosPermitidos.Contains(extension))
            {
                ModelState.AddModelError(string.Empty, "El archivo seleccionado no tiene un formato de imagen válido. Use JPG, JPEG o PNG.");
                return View(vm);
            }

            try
            {
                using var stream = vm.ImagenCedula.OpenReadStream();
                await _votacionService.ValidarOcrAsync(sesion.CedulaIngresada, stream);
                await _votacionService.GenerarYEnviarCodigoAsync(sesion.CiudadanoId, sesion.EleccionId);

                sesion.PasoActual = PasoVotacion.OcrValidado;
                HttpContext.Session.Set(SesionVotacionKey, sesion);

                return RedirectToAction(nameof(VerificarCodigo));
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // ─── PASO 3: VERIFICACIÓN DE CÓDIGO ──────────────────────────────────────

        [HttpGet]
        public IActionResult VerificarCodigo()
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.OcrValidado)
                return RedirectToAction(nameof(ValidarIdentidad));

            ViewData["PasoActivo"] = 3;
            var vm = new VerificacionCodigoViewModel
            {
                CiudadanoId             = sesion.CiudadanoId,
                EleccionId              = sesion.EleccionId,
                CorreoElectronicoOculto = OcultarCorreo(sesion.CorreoElectronico)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarCodigo(VerificacionCodigoViewModel vm)
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.OcrValidado)
                return RedirectToAction(nameof(ValidarIdentidad));

            ViewData["PasoActivo"] = 3;
            if (!ModelState.IsValid)
            {
                vm.CorreoElectronicoOculto = OcultarCorreo(sesion.CorreoElectronico);
                return View(vm);
            }

            try
            {
                await _votacionService.ValidarCodigoVerificacionAsync(sesion.CiudadanoId, sesion.EleccionId, vm.Codigo);

                sesion.PasoActual = PasoVotacion.CodigoValidado;
                HttpContext.Session.Set(SesionVotacionKey, sesion);

                return RedirectToAction(nameof(Boleta));
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.CorreoElectronicoOculto = OcultarCorreo(sesion.CorreoElectronico);
                return View(vm);
            }
        }

        // ─── PASO 4: BOLETA ELECTORAL ─────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Boleta()
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.CodigoValidado)
                return RedirectToAction(nameof(ValidarIdentidad));

            ViewData["PasoActivo"] = 4;
            var boletaDto = await _votacionService.ObtenerBoletaElectoralAsync(sesion.EleccionId);

            var vm = new BoletaElectoralViewModel
            {
                CiudadanoId = sesion.CiudadanoId,
                VotanteId   = sesion.CiudadanoId,
                EleccionId  = sesion.EleccionId,
                Puestos     = boletaDto.Select(p => new PuestoBoletaViewModel
                {
                    PuestoId     = p.PuestoId,
                    PuestoNombre = p.PuestoNombre,
                    Candidatos   = p.Candidatos.Select(c => new CandidatoBoletaViewModel
                    {
                        CandidatoId       = c.CandidatoId,
                        CandidatoNombre   = c.NombreCompleto,
                        FotoCandidatoUrl  = c.FotoUrl,
                        PartidoPoliticoId = c.PartidoPoliticoId,
                        PartidoNombre     = c.PartidoNombre,
                        LogoPartidoUrl    = c.LogoPartido
                    }).ToList()
                }).ToList()
            };

            // Restaurar selecciones previas si el elector volvió desde la revisión
            if (sesion.SelectionsPorPuesto != null)
            {
                foreach (var puesto in vm.Puestos)
                {
                    if (sesion.SelectionsPorPuesto.TryGetValue(puesto.PuestoId, out var candidatoId))
                        puesto.CandidatoSeleccionadoId = candidatoId;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revisar(BoletaElectoralViewModel vm)
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.CodigoValidado)
                return RedirectToAction(nameof(ValidarIdentidad));

            ViewData["PasoActivo"] = 4; // Mantener en paso 4 durante la revisión

            // Puestos sin selección → voto en blanco (CandidatoSeleccionadoId = null o vacío)
            // Poblar candidatos desde la boleta para poder renderizar si hay error
            if (vm.Puestos == null || !vm.Puestos.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe realizar una selección para todos los puestos electivos antes de continuar.");
                await RepoblarCandidatos(vm, sesion.EleccionId);
                return View("Boleta", vm);
            }

            // Guardar selecciones en sesión (null = blanco, candidatoId = votó por candidato)
            sesion.SelectionsPorPuesto = vm.Puestos
                .ToDictionary(p => p.PuestoId, p => p.CandidatoSeleccionadoId);
            sesion.PasoActual = PasoVotacion.Revisando;
            HttpContext.Session.Set(SesionVotacionKey, sesion);

            // Construir resumen enriquecido para la vista de revisión
            var boletaDto = await _votacionService.ObtenerBoletaElectoralAsync(sesion.EleccionId);
            var resumen = ConstruirResumen(vm.Puestos, boletaDto);

            var vmConfirmacion = new ConfirmacionVotoViewModel
            {
                VotosSeleccionados = resumen
            };

            return View("ConfirmacionVoto", vmConfirmacion);
        }

        // ─── PASO 5: CONFIRMAR VOTO FINAL ─────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarVotoFinal()
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.Revisando)
                return RedirectToAction(nameof(ValidarIdentidad));

            ViewData["PasoActivo"] = 5;

            try
            {
                var votosDto = (sesion.SelectionsPorPuesto ?? [])
                    .Select(kv => new VotoDto
                    {
                        EleccionId       = sesion.EleccionId,
                        PuestoElectivoId = kv.Key,
                        // null o valor negativo = voto en blanco
                        CandidatoId      = (kv.Value.HasValue && kv.Value.Value > 0) ? kv.Value : null,
                        PartidoPoliticoId = null
                    });

                await _votacionService.ProcesarVotacionAsync(sesion.CiudadanoId, sesion.EleccionId, votosDto);

                // Limpiar sesión tras voto exitoso
                HttpContext.Session.Remove(SesionVotacionKey);

                return RedirectToAction(nameof(VotoExitoso));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error procesando su votación. Por favor, reintente.");
                return RedirectToAction(nameof(ValidarIdentidad));
            }
        }

        // ─── PANTALLA DE ÉXITO ────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult VotoExitoso()
        {
            ViewData["PasoActivo"] = 5;
            var vm = new ConfirmacionVotoViewModel
            {
                FechaParticipacion = DateTime.Now
            };
            return View(vm);
        }

        // ─── CONFIRMACIÓN (legacy GET — redirige a VotoExitoso) ──────────────────

        [HttpGet]
        public IActionResult ConfirmacionVoto()
        {
            return RedirectToAction(nameof(VotoExitoso));
        }

        // ─── HELPERS ──────────────────────────────────────────────────────────────

        private async Task RepoblarCandidatos(BoletaElectoralViewModel vm, int eleccionId)
        {
            var boletaDto = await _votacionService.ObtenerBoletaElectoralAsync(eleccionId);
            if (vm.Puestos == null) return;
            foreach (var puesto in vm.Puestos)
            {
                var puestoDto = boletaDto.FirstOrDefault(p => p.PuestoId == puesto.PuestoId);
                if (puestoDto != null)
                {
                    puesto.Candidatos = puestoDto.Candidatos.Select(c => new CandidatoBoletaViewModel
                    {
                        CandidatoId       = c.CandidatoId,
                        CandidatoNombre   = c.NombreCompleto,
                        FotoCandidatoUrl  = c.FotoUrl,
                        PartidoPoliticoId = c.PartidoPoliticoId,
                        PartidoNombre     = c.PartidoNombre,
                        LogoPartidoUrl    = c.LogoPartido
                    }).ToList();
                }
            }
        }

        private static IEnumerable<ResumenSeleccionViewModel> ConstruirResumen(
            List<PuestoBoletaViewModel> puestos,
            IEnumerable<eVote360Pro.Application.DTOs.PuestoBoletaDto> boletaDto)
        {
            var resultado = new List<ResumenSeleccionViewModel>();
            foreach (var puesto in puestos)
            {
                var puestoDto = boletaDto.FirstOrDefault(p => p.PuestoId == puesto.PuestoId);
                var item = new ResumenSeleccionViewModel { PuestoNombre = puesto.PuestoNombre };

                if (puesto.CandidatoSeleccionadoId.HasValue && puesto.CandidatoSeleccionadoId.Value > 0)
                {
                    var candidatoDto = puestoDto?.Candidatos
                        .FirstOrDefault(c => c.CandidatoId == puesto.CandidatoSeleccionadoId.Value);

                    if (candidatoDto != null)
                    {
                        item.CandidatoNombreCompleto = candidatoDto.NombreCompleto;
                        item.FotoCandidato           = candidatoDto.FotoUrl;
                        item.PartidoNombre           = candidatoDto.PartidoNombre;
                        item.LogoPartido             = candidatoDto.LogoPartido;
                    }
                }

                resultado.Add(item);
            }
            return resultado;
        }

        private static string OcultarCorreo(string? correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return "correo registrado";
            var partes = correo.Split('@');
            if (partes.Length != 2) return "****@****";
            var usuario = partes[0];
            var visible = usuario.Length > 2 ? usuario[..2] : usuario;
            return $"{visible}***@{partes[1]}";
        }
    }

    // ─── MODELO DE SESIÓN ────────────────────────────────────────────────────────

    public class VotacionSesionData
    {
        public int CiudadanoId { get; set; }
        public int EleccionId { get; set; }
        public string CedulaIngresada { get; set; } = string.Empty;
        public string? CorreoElectronico { get; set; }
        public PasoVotacion PasoActual { get; set; }
        /// <summary>PuestoId → CandidatoId (null = voto en blanco)</summary>
        public Dictionary<int, int?>? SelectionsPorPuesto { get; set; }
    }

    public enum PasoVotacion
    {
        Inicio            = 0,
        IdentidadValidada = 1,
        OcrValidado       = 2,
        CodigoValidado    = 3,
        Revisando         = 4,
        VotoEmitido       = 5
    }
}
