using Gdac.Auth.Application.Features.Auth.Commands.ChangePassword;
using Gdac.Auth.Application.Features.Auth.Commands.ForgotPassword;
using Gdac.Auth.Application.Features.Auth.Commands.Login;
using Gdac.Auth.Application.Features.Auth.Commands.Logout;
using Gdac.Auth.Application.Features.Auth.Commands.LogoutAll;
using Gdac.Auth.Application.Features.Auth.Commands.Refresh;
using Gdac.Auth.Application.Features.Auth.Commands.ResetPassword;
using Gdac.Auth.Application.Features.Auth.Queries.Introspect;
using Gdac.Auth.Api.Extensions;
using Gdac.Auth.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gdac.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var device = Request.Headers.UserAgent.ToString().Truncate(500);

        var result = await mediator.Send(new LoginCommand(
            request.Email, request.Password, request.ClientId, request.ClientSecret, ip, device), ct);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var accessToken = GetBearerToken();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await mediator.Send(new RefreshCommand(accessToken, request.RefreshToken, ip), ct);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var sessionId = GetSessionId();
        await mediator.Send(new LogoutCommand(sessionId), ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        var userId = GetUserId();
        await mediator.Send(new LogoutAllCommand(userId), ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        var sessionId = GetSessionId();

        await mediator.Send(new ChangePasswordCommand(
            userId, sessionId, request.CurrentPassword, request.NewPassword, request.ConfirmPassword), ct);

        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await mediator.Send(new ForgotPasswordCommand(request.Email), ct);
        return NoContent();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        await mediator.Send(new ResetPasswordCommand(request.Token, request.NewPassword, request.ConfirmPassword), ct);
        return NoContent();
    }

    [HttpPost("introspect")]
    public async Task<IActionResult> Introspect(CancellationToken ct)
    {
        var accessToken = GetBearerToken();
        var clientId = Request.Headers["X-Client-Id"].ToString();
        var clientSecret = Request.Headers["X-Client-Secret"].ToString();

        var result = await mediator.Send(new IntrospectQuery(accessToken, clientId, clientSecret), ct);
        return Ok(result);
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

    private Guid GetSessionId() =>
        Guid.Parse(User.FindFirstValue(GdacClaimTypes.SessionId)!);

    private string GetBearerToken() =>
        HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
}

// DTOs de request

public record LoginRequest(string Email, string Password, string ClientId, string ClientSecret);
public record RefreshRequest(string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword, string ConfirmPassword);
