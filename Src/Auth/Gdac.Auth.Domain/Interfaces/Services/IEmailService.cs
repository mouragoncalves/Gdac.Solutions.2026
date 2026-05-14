namespace Gdac.Auth.Domain.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetAsync(string toEmail, string resetToken, CancellationToken ct = default);
    Task SendTemporaryPasswordAsync(string toEmail, string temporaryPassword, CancellationToken ct = default);
}
