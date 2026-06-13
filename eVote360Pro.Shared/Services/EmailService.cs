using eVote360Pro.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace eVote360Pro.Shared.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(EmailSettings settings) => _settings = settings;

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_settings.NombreRemitente, _settings.CorreoRemitente));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;

        // Se asume que el cuerpo viene ya formateado por quien llama al servicio
        mensaje.Body = new BodyBuilder { HtmlBody = cuerpo }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Puerto, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.CorreoRemitente, _settings.Password);
        await client.SendAsync(mensaje);
        await client.DisconnectAsync(true);
    }
}