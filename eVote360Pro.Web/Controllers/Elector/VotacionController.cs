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

        public VotacionController(IVotacionService votacionService)
        {
            _votacionService = votacionService;
        }

        // Pantalla de Bienvenida

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Validamos si hay una elección abierta antes de permitir avanzar
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

        // PASO 1: VALIDACIÓN DE IDENTIDAD (CÉDULA)
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
                // Validamos que la cédula sea correcta y que no haya registros de votación previos
                var ciudadano = await _votacionService.ValidarCiudadanoParaVotarAsync(vm.Cedula, eleccion.Id);

                // Si la identidad pasa los filtros, enviamos el código de 6 dígitos
                await _votacionService.GenerarYEnviarCodigoAsync(ciudadano.Id, eleccion.Id);

                return RedirectToAction(nameof(VerificarCodigo), new { ciudadanoId = ciudadano.Id, eleccionId = eleccion.Id });
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // PASO 2: VERIFICACIÓN DE CÓDIGO
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
                // Confirmamos que el código coincida, esté activo y no haya expirado
                await _votacionService.ValidarCodigoVerificacionAsync(vm.CiudadanoId, vm.EleccionId, vm.Codigo);

                // Acceso concedido a las boletas oficiales
                return RedirectToAction(nameof(Boleta), new { ciudadanoId = vm.CiudadanoId, eleccionId = vm.EleccionId });
            }
            catch (ValidacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        // PASO 3: BOLETA ELECTORAL (EMISIÓN DEL VOTO)
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

                // Recargamos el catálogo visual de la boleta si ocurre un Rollback en base de datos
                var boletaDto = await _votacionService.ObtenerBoletaElectoralAsync(vm.EleccionId);
                ViewBag.BoletaElectoral = boletaDto;
                return View(vm);
            }
        }

        // CONFIRMACIÓN DEL VOTO
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