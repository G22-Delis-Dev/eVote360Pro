using eVote360Pro.Application.DTOs;
using eVote360Pro.Application.Interfaces;
using eVote360Pro.Domain.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace eVote360Pro.Shared.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value; 
    }

    public async Task EnviarCodigoVerificacionAsync(string destinatario, string nombreCiudadano, string codigo)
    {
        var asunto = "Código de verificación - eVote360 Pro";
        var cuerpo = $@"<p>Hola {nombreCiudadano}, tu código es: <strong>{codigo}</strong></p>";
        await EnviarAsync(destinatario, asunto, cuerpo);
    }

    public async Task EnviarResumenVotacionAsync(string destinatario, string nombreCiudadano, ResumenVotacionDto resumen)
    {
        var asunto = $"Resumen de votación - {resumen.NombreEleccion}";
        var cuerpo = $@"<p>Hola {nombreCiudadano}, tu votación fue exitosa.</p>";
        await EnviarAsync(destinatario, asunto, cuerpo);
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