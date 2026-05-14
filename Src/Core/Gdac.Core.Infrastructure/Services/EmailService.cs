using Gdac.Core.Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Gdac.Core.Infrastructure.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        var host      = configuration["Email:Host"]!;
        var port      = int.Parse(configuration["Email:Port"] ?? "587");
        var username  = configuration["Email:Username"]!;
        var password  = configuration["Email:Password"]!;
        var fromEmail = configuration["Email:From"] ?? username;
        var fromName  = configuration["Email:FromName"] ?? "GDAC";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body    = new TextPart("html") { Text = htmlBody };

        var socketOptions = ParseSslMode(configuration["Email:SslMode"]);

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, socketOptions, ct);
        if (socketOptions != MailKit.Security.SecureSocketOptions.None)
            await client.AuthenticateAsync(username, password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("E-mail enviado para {To} | Assunto: {Subject}", toEmail, subject);
    }

    private static MailKit.Security.SecureSocketOptions ParseSslMode(string? mode) => mode switch
    {
        "SslOnConnect" => MailKit.Security.SecureSocketOptions.SslOnConnect,
        "StartTls"     => MailKit.Security.SecureSocketOptions.StartTls,
        "None"         => MailKit.Security.SecureSocketOptions.None,
        _              => MailKit.Security.SecureSocketOptions.Auto
    };
}
