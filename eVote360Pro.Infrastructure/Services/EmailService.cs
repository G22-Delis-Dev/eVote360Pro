using eVote360Pro.Domain.Settings;
using eVote360Pro.Shared.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace eVote360Pro.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
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