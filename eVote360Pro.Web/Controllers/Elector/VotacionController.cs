using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Application.ViewModels.Votacion;
using eVote360Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers.Elector
{
    public class VotacionController : Controller
    {
        private readonly IVotacionService _votacionService;

        private static readonly string[] FormatosPermitidos = [".jpg", ".jpeg", ".png"];

        public VotacionController(IVotacionService votacionService)
        {
            _votacionService = votacionService;
        }

        // ─── PANTALLA DE BIENVENIDA ───────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var eleccionActiva = await _votacionService.ObtenerEleccionActivaAsync();
            if (eleccionActiva == null)
            {
                return View("SinElecciones");
            }

            var vm = new InicioVotacionViewModel
            {
                EleccionId = eleccionActiva.Id,
                EleccionNombre = eleccionActiva.Nombre,
                FechaRealizacion = eleccionActiva.FechaRealizacion
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

            var eleccion = await _votacionService.ObtenerEleccionActivaAsync();
            if (eleccion == null) return RedirectToAction(nameof(Index));

            try
            {
                // Validar que el ciudadano existe, está activo y no ha votado
                var ciudadano = await _votacionService.ValidarCiudadanoParaVotarAsync(vm.Cedula, eleccion.Id);

                // Redirigir al paso OCR con los datos necesarios
                return RedirectToAction(nameof(ValidarOcr), new
                {
                    ciudadanoId = ciudadano.Id,
                    eleccionId = eleccion.Id,
                    cedulaIngresada = vm.Cedula
                });
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // ─── PASO 2: VALIDACIÓN OCR (IMAGEN DE CÉDULA) ───────────────────────────

        [HttpGet]
        public IActionResult ValidarOcr(int ciudadanoId, int eleccionId, string cedulaIngresada)
        {
            var vm = new ValidacionOcrViewModel
            {
                CiudadanoId = ciudadanoId,
                EleccionId = eleccionId,
                CedulaIngresada = cedulaIngresada
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarOcr(ValidacionOcrViewModel vm)
        {
            // Validar que se subió un archivo
            if (vm.ImagenCedula == null || vm.ImagenCedula.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe subir una imagen de su cédula para validar su identidad.");
                return View(vm);
            }

            // Validar el formato del archivo
            var extension = Path.GetExtension(vm.ImagenCedula.FileName).ToLowerInvariant();
            if (!FormatosPermitidos.Contains(extension))
            {
                ModelState.AddModelError(string.Empty, "El archivo seleccionado no tiene un formato de imagen válido.");
                return View(vm);
            }

            try
            {
                // Procesar OCR y validar coincidencia con la cédula ingresada
                using var stream = vm.ImagenCedula.OpenReadStream();
                await _votacionService.ValidarOcrAsync(vm.CedulaIngresada, stream);

                // OCR exitoso: generar y enviar código por correo
                await _votacionService.GenerarYEnviarCodigoAsync(vm.CiudadanoId, vm.EleccionId);

                // Obtener correo para mostrarlo oculto en la siguiente pantalla
                return RedirectToAction(nameof(VerificarCodigo), new
                {
                    ciudadanoId = vm.CiudadanoId,
                    eleccionId = vm.EleccionId
                });
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // ─── PASO 3: VERIFICACIÓN DE CÓDIGO POR CORREO ───────────────────────────

        [HttpGet]
        public IActionResult VerificarCodigo(int ciudadanoId, int eleccionId)
        {
            var vm = new VerificacionCodigoViewModel
            {
                CiudadanoId = ciudadanoId,
                EleccionId = eleccionId
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarCodigo(VerificacionCodigoViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                // Confirmar que el código es válido, no ha expirado y no fue usado
                await _votacionService.ValidarCodigoVerificacionAsync(vm.CiudadanoId, vm.EleccionId, vm.Codigo);

                // Acceso concedido a la boleta electoral
                return RedirectToAction(nameof(Boleta), new { ciudadanoId = vm.CiudadanoId, eleccionId = vm.EleccionId });
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // ─── PASO 4: BOLETA ELECTORAL ─────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Boleta(int ciudadanoId, int eleccionId)
        {
            var boletaDto = await _votacionService.ObtenerBoletaElectoralAsync(eleccionId);

            ViewBag.BoletaElectoral = boletaDto;

            var vm = new BoletaElectoralViewModel
            {
                CiudadanoId = ciudadanoId,
                EleccionId = eleccionId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Boleta(BoletaElectoralViewModel vm)
        {
            try
            {
                var votosDto = vm.Selecciones.Select(s => new VotoDto
                {
                    EleccionId = vm.EleccionId,
                    PuestoElectivoId = s.PuestoElectivoId,
                    CandidatoId = s.CandidatoId,
                    PartidoPoliticoId = s.PartidoPoliticoId
                });

                await _votacionService.ProcesarVotacionAsync(vm.CiudadanoId, vm.EleccionId, votosDto);

                return RedirectToAction(nameof(ConfirmacionVoto));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error crítico de consistencia procesando su votación. Por favor, reintente.");

                var boletaDto = await _votacionService.ObtenerBoletaElectoralAsync(vm.EleccionId);
                ViewBag.BoletaElectoral = boletaDto;
                return View(vm);
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
    }
}