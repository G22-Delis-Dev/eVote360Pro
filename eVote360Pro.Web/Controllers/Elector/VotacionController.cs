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

        // Clave de sesión para el estado del flujo de votación
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
            // Limpiar cualquier sesión de votación anterior al iniciar
            HttpContext.Session.Remove(SesionVotacionKey);

            var eleccionActiva = await _votacionService.ObtenerEleccionActivaAsync();
            if (eleccionActiva == null)
                return View("SinElecciones");

            var vm = new InicioVotacionViewModel
            {
                EleccionId          = eleccionActiva.Id,
                EleccionNombre      = eleccionActiva.Nombre,
                FechaRealizacion    = eleccionActiva.FechaRealizacion
            };

            return View(vm);
        }

        // ─── PASO 1: VALIDACIÓN DE CÉDULA ────────────────────────────────────────

        [HttpGet]
        public IActionResult ValidarIdentidad()
        {
            return View(new ValidacionIdentidadViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarIdentidad(ValidacionIdentidadViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Normalizar cédula: quitar guiones y espacios innecesarios
            vm.NumeroDocumento = vm.NumeroDocumento.Replace("-", "").Replace(" ", "").Trim();

            var eleccion = await _votacionService.ObtenerEleccionActivaAsync();
            if (eleccion == null) return RedirectToAction(nameof(Index));

            try
            {
                var ciudadano = await _votacionService.ValidarCiudadanoParaVotarAsync(vm.Cedula, eleccion.Id);

                // Guardar estado en sesión — ya no viaja en la URL
                var sesion = new VotacionSesionData
                {
                    CiudadanoId      = ciudadano.Id,
                    EleccionId       = eleccion.Id,
                    CedulaIngresada  = vm.Cedula,
                    CorreoElectronico = ciudadano.CorreoElectronico,
                    PasoActual       = PasoVotacion.IdentidadValidada
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

            if (vm.ImagenCedula == null || vm.ImagenCedula.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe subir una imagen de su cédula para validar su identidad.");
                vm.CiudadanoId     = sesion.CiudadanoId;
                vm.EleccionId      = sesion.EleccionId;
                vm.CedulaIngresada = sesion.CedulaIngresada;
                return View(vm);
            }

            var extension = Path.GetExtension(vm.ImagenCedula.FileName).ToLowerInvariant();
            if (!FormatosPermitidos.Contains(extension))
            {
                ModelState.AddModelError(string.Empty, "El archivo seleccionado no tiene un formato de imagen válido. Use JPG, JPEG o PNG.");
                vm.CiudadanoId     = sesion.CiudadanoId;
                vm.EleccionId      = sesion.EleccionId;
                vm.CedulaIngresada = sesion.CedulaIngresada;
                return View(vm);
            }

            try
            {
                using var stream = vm.ImagenCedula.OpenReadStream();
                await _votacionService.ValidarOcrAsync(sesion.CedulaIngresada, stream);

                await _votacionService.GenerarYEnviarCodigoAsync(sesion.CiudadanoId, sesion.EleccionId);

                // Avanzar paso en sesión
                sesion.PasoActual = PasoVotacion.OcrValidado;
                HttpContext.Session.Set(SesionVotacionKey, sesion);

                return RedirectToAction(nameof(VerificarCodigo));
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.CiudadanoId     = sesion.CiudadanoId;
                vm.EleccionId      = sesion.EleccionId;
                vm.CedulaIngresada = sesion.CedulaIngresada;
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

            var vm = new VerificacionCodigoViewModel
            {
                CiudadanoId            = sesion.CiudadanoId,
                EleccionId             = sesion.EleccionId,
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

            var boletaDto = await _votacionService.ObtenerBoletaElectoralAsync(sesion.EleccionId);

            var vm = new BoletaElectoralViewModel
            {
                CiudadanoId = sesion.CiudadanoId,
                VotanteId   = sesion.CiudadanoId,
                EleccionId  = sesion.EleccionId,
                Puestos     = boletaDto.Select(p => new PuestoBoletaViewModel
                {
                    PuestoId    = p.PuestoId,
                    PuestoNombre = p.PuestoNombre,
                    Candidatos  = p.Candidatos.Select(c => new CandidatoBoletaViewModel
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

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Boleta(BoletaElectoralViewModel vm)
        {
            var sesion = HttpContext.Session.Get<VotacionSesionData>(SesionVotacionKey);
            if (sesion == null || sesion.PasoActual < PasoVotacion.CodigoValidado)
                return RedirectToAction(nameof(ValidarIdentidad));

            // Validar que todos los puestos tienen selección (-1 = voto en blanco, es válido)
            if (vm.Puestos == null || !vm.Puestos.Any() ||
                vm.Puestos.Any(p => !p.CandidatoSeleccionadoId.HasValue))
            {
                ModelState.AddModelError(string.Empty, "Debe realizar una selección para todos los puestos electivos antes de continuar.");
                await RepoblarCandidatos(vm, sesion.EleccionId);
                return View(vm);
            }

            try
            {
                // -1 indica voto en blanco (Ninguno), se convierte a null al guardar
                var votosDto = vm.Puestos.Select(p => new VotoDto
                {
                    EleccionId        = sesion.EleccionId,
                    PuestoElectivoId  = p.PuestoId,
                    CandidatoId       = p.CandidatoSeleccionadoId == -1 ? null : p.CandidatoSeleccionadoId,
                    PartidoPoliticoId = null // anónimo — se resuelve por candidato en la capa de servicio
                });

                await _votacionService.ProcesarVotacionAsync(sesion.CiudadanoId, sesion.EleccionId, votosDto);

                // Limpiar sesión de votación al finalizar
                HttpContext.Session.Remove(SesionVotacionKey);

                return RedirectToAction(nameof(ConfirmacionVoto));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error procesando su votación. Por favor, reintente.");
                await RepoblarCandidatos(vm, sesion.EleccionId);
                return View(vm);
            }
        }

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
                        CandidatoId      = c.CandidatoId,
                        CandidatoNombre  = c.NombreCompleto,
                        FotoCandidatoUrl = c.FotoUrl,
                        PartidoPoliticoId = c.PartidoPoliticoId,
                        PartidoNombre    = c.PartidoNombre,
                        LogoPartidoUrl   = c.LogoPartido
                    }).ToList();
                }
            }
        }

        // ─── CONFIRMACIÓN DEL VOTO ────────────────────────────────────────────────

        [HttpGet]
        public IActionResult ConfirmacionVoto()
        {
            var vm = new ConfirmacionVotoViewModel
            {
                FechaParticipacion = DateTime.Now
            };
            return View(vm);
        }

        // ─── HELPERS ──────────────────────────────────────────────────────────────

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

    // ─── MODELO DE SESIÓN (solo para este flujo) ──────────────────────────────

    public class VotacionSesionData
    {
        public int CiudadanoId { get; set; }
        public int EleccionId { get; set; }
        public string CedulaIngresada { get; set; } = string.Empty;
        public string? CorreoElectronico { get; set; }
        public PasoVotacion PasoActual { get; set; }
    }

    public enum PasoVotacion
    {
        Inicio            = 0,
        IdentidadValidada = 1,
        OcrValidado       = 2,
        CodigoValidado    = 3,
        VotoEmitido       = 4
    }
}
