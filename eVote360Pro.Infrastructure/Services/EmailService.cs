using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Settings;
using eVote360Pro.Shared.Interfaces; // Referencia a la interfaz en Shared
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace eVote360Pro.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly IEmailTemplateService _templateService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, IEmailTemplateService templateService, ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task EnviarCodigoVerificacionAsync(string destinatario, string nombreCiudadano, string codigo)
    {
        var cuerpo = _templateService.GenerarCodigoVerificacionHtml(nombreCiudadano, codigo);
        await EnviarAsync(destinatario, "Código de verificación - eVote360 Pro", cuerpo);
    }

    // Nota: El parámetro resumenDto se trata como object para evitar dependencia de Shared a Application
    public async Task EnviarResumenVotacionAsync(string destinatario, string nombreCiudadano, object resumen)
    {
        var dto = (ResumenVotacionDto)resumen;
        var cuerpo = _templateService.GenerarResumenVotacionHtml(nombreCiudadano, dto);
        await EnviarAsync(destinatario, $"Resumen de votación - {dto.NombreEleccion}", cuerpo);
    }

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_settings.NombreRemitente, _settings.CorreoRemitente));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;
        mensaje.Body = new BodyBuilder { HtmlBody = cuerpoHtml }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Puerto, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.CorreoRemitente, _settings.Password);
        await client.SendAsync(mensaje);
        await client.DisconnectAsync(true);
    }
}