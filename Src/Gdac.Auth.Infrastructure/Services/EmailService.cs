using Gdac.Auth.Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Gdac.Auth.Infrastructure.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendPasswordResetAsync(string toEmail, string resetToken, CancellationToken ct = default)
    {
        var baseUrl = configuration["Email:BaseUrl"] ?? "https://auth.gdac.com.br";
        var resetUrl = $"{baseUrl}/redefinir-senha?token={Uri.EscapeDataString(resetToken)}";

        var body = $"""
            <p>Você solicitou a redefinição da sua senha.</p>
            <p>Clique no link abaixo para criar uma nova senha. O link expira em 30 minutos.</p>
            <p><a href="{resetUrl}">Redefinir senha</a></p>
            <p>Se você não solicitou a redefinição, ignore este e-mail.</p>
            """;

        await SendAsync(toEmail, "Redefinição de senha — GDAC", body, ct);
    }

    public async Task SendTemporaryPasswordAsync(string toEmail, string temporaryPassword, CancellationToken ct = default)
    {
        var body = $"""
            <p>Sua conta GDAC foi criada.</p>
            <p>Sua senha temporária é: <strong>{temporaryPassword}</strong></p>
            <p>Você será obrigado a criar uma nova senha no primeiro acesso.</p>
            """;

        await SendAsync(toEmail, "Bem-vindo ao GDAC — Sua conta foi criada", body, ct);
    }

    private async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct)
    {
        var host = configuration["Email:Host"]!;
        var port = int.Parse(configuration["Email:Port"] ?? "587");
        var username = configuration["Email:Username"]!;
        var password = configuration["Email:Password"]!;
        var fromEmail = configuration["Email:From"] ?? username;
        var fromName = configuration["Email:FromName"] ?? "GDAC";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        var useSsl = bool.Parse(configuration["Email:UseSsl"] ?? "true");
        var socketOptions = useSsl
            ? MailKit.Security.SecureSocketOptions.StartTls
            : MailKit.Security.SecureSocketOptions.None;

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, socketOptions, ct);
        if (useSsl)
            await client.AuthenticateAsync(username, password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("E-mail enviado para {To} | Assunto: {Subject}", toEmail, subject);
    }
}
