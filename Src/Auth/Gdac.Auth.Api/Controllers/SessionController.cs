using Gdac.Auth.Application.Features.Session.Queries.GetApps;
using Gdac.Auth.Application.Features.Session.Queries.GetCompanies;
using Gdac.Auth.Application.Features.Session.Queries.GetMe;
using Gdac.Auth.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gdac.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/session")]
[Authorize]
public class SessionController(IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = GetUserId();
        var sessionId = GetSessionId();
        var result = await mediator.Send(new GetMeQuery(userId, sessionId), ct);
        return Ok(result);
    }

    [HttpGet("apps")]
    public async Task<IActionResult> Apps(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAppsQuery(GetUserId()), ct);
        return Ok(result);
    }

    [HttpGet("companies")]
    public async Task<IActionResult> Companies(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCompaniesQuery(GetUserId()), ct);
        return Ok(result);
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

    private Guid GetSessionId() =>
        Guid.Parse(User.FindFirstValue(GdacClaimTypes.SessionId)!);
}
