using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace eVote360Pro.Shared.Services;

public interface IEmailService
{
    Task EnviarCodigoVerificacionAsync(string destinatario, string nombreCiudadano, string codigo);
    Task EnviarResumenVotacionAsync(string destinatario, string nombreCiudadano, ResumenVotacionDto resumen);
}

public class ResumenVotacionDto
{
    public string NombreEleccion { get; set; } = string.Empty;
    public DateTime FechaEleccion { get; set; }
    public List<VotoResumenDto> Votos { get; set; } = [];
}

public class VotoResumenDto
{
    public string Puesto { get; set; } = string.Empty;
    public string Candidato { get; set; } = string.Empty;
    public string Partido { get; set; } = string.Empty;
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(EmailSettings settings)
    {
        _settings = settings;
    }

    public async Task EnviarCodigoVerificacionAsync(
        string destinatario,
        string nombreCiudadano,
        string codigo)
    {
        var asunto = "Código de verificación - eVote360 Pro";

        var cuerpo = $"""
            <html>
            <body style="font-family: Arial, sans-serif; color: #333;">
                <h2>Verificación de identidad</h2>
                <p>Hola {nombreCiudadano},</p>
                <p>Tu código de verificación para continuar con el proceso de votación es:</p>
                <div style="font-size: 32px; font-weight: bold; letter-spacing: 8px;
                            padding: 16px; background:#f4f4f4; text-align:center;
                            border-radius: 8px; margin: 16px 0;">
                    {codigo}
                </div>
                <p>Este código tiene una vigencia de <strong>5 minutos</strong>.</p>
                <p>Si no solicitaste este código, ignora este mensaje.</p>
                <br/>
                <p>eVote360 Pro</p>
            </body>
            </html>
            """;

        await EnviarAsync(destinatario, asunto, cuerpo);
    }

    public async Task EnviarResumenVotacionAsync(
        string destinatario,
        string nombreCiudadano,
        ResumenVotacionDto resumen)
    {
        var asunto = $"Resumen de votación - {resumen.NombreEleccion}";

        var filasVotos = string.Join("", resumen.Votos.Select(v => $"""
            <tr>
                <td style="padding: 8px; border: 1px solid #ddd;">{v.Puesto}</td>
                <td style="padding: 8px; border: 1px solid #ddd;">{v.Candidato}</td>
                <td style="padding: 8px; border: 1px solid #ddd;">{v.Partido}</td>
            </tr>
            """));

        var cuerpo = $"""
            <html>
            <body style="font-family: Arial, sans-serif; color: #333;">
                <h2>Resumen de tu votación</h2>
                <p>Hola {nombreCiudadano},</p>
                <p>Has completado exitosamente tu proceso de votación.</p>
                <br/>
                <p><strong>Elección:</strong> {resumen.NombreEleccion}</p>
                <p><strong>Fecha:</strong> {resumen.FechaEleccion:dd/MM/yyyy}</p>
                <br/>
                <table style="border-collapse: collapse; width: 100%;">
                    <thead>
                        <tr style="background-color: #f4f4f4;">
                            <th style="padding: 8px; border: 1px solid #ddd; text-align:left;">Puesto</th>
                            <th style="padding: 8px; border: 1px solid #ddd; text-align:left;">Candidato</th>
                            <th style="padding: 8px; border: 1px solid #ddd; text-align:left;">Partido</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filasVotos}
                    </tbody>
                </table>
                <br/>
                <p>Gracias por ejercer tu derecho al voto.</p>
                <p>eVote360 Pro</p>
            </body>
            </html>
            """;

        await EnviarAsync(destinatario, asunto, cuerpo);
    }

    private async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_settings.NombreRemitente, _settings.CorreoRemitente));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;

        var builder = new BodyBuilder { HtmlBody = cuerpoHtml };
        mensaje.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_settings.Host, _settings.Puerto, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.CorreoRemitente, _settings.Password);
        await client.SendAsync(mensaje);
        await client.DisconnectAsync(true);
    }
}