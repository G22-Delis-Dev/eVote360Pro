using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Web.Controllers
{
    public class VotacionController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public VotacionController(IEmailService emailService, IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task EnviarNotificacion(string email, string nombre, ResumenVotacionDto resumen)
        {
            // 1. Generar HTML usando el servicio de plantillas
            var html = _templateService.GenerarResumenVotacionHtml(nombre, resumen);

            // 2. Enviar usando el servicio de correo
            await _emailService.EnviarAsync(email, "Resumen de Voto", html);
        }
    }
}
