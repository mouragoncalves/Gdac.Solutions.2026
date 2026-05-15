using Gdac.Onboarding.Application.Features.Registrations.Commands.ApproveRegistration;
using Gdac.Onboarding.Application.Features.Registrations.Commands.RejectRegistration;
using Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitClientRegistration;
using Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitPartnerRegistration;
using Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistration;
using Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistrations;
using Gdac.Onboarding.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Onboarding.Api.Controllers;

[ApiController]
[Route("registrations")]
public class RegistrationController(IMediator mediator) : ControllerBase
{
    [HttpPost("client")]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitClient([FromBody] SubmitClientRequest request, CancellationToken ct)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var id = await mediator.Send(new SubmitClientRegistrationCommand(
            request.ContactName, request.ContactEmail, request.ContactPhone,
            request.CompanyName, request.TradeName, request.Cnpj,
            request.Segment, request.SizeCategory,
            request.City, request.State, request.ReferralCode,
            ipAddress), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("partner")]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitPartner([FromBody] SubmitPartnerRequest request, CancellationToken ct)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var id = await mediator.Send(new SubmitPartnerRegistrationCommand(
            request.ContactName, request.ContactEmail, request.ContactPhone,
            request.CompanyName, request.TradeName, request.Cnpj,
            request.Segment, request.SizeCategory,
            request.City, request.State,
            ipAddress), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] RegistrationStatus? status, CancellationToken ct) =>
        Ok(await mediator.Send(new GetRegistrationsQuery(status), ct));

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetRegistrationQuery(id), ct));

    [HttpPost("{id:guid}/approve")]
    [Authorize]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ReviewRequest request, CancellationToken ct)
    {
        var reviewedBy = GetCurrentUserId();
        var result = await mediator.Send(new ApproveRegistrationCommand(id, reviewedBy, request.Notes), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ReviewRequest request, CancellationToken ct)
    {
        var reviewedBy = GetCurrentUserId();
        await mediator.Send(new RejectRegistrationCommand(id, reviewedBy, request.Notes), ct);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }
}

public record SubmitClientRequest(
    string ContactName, string ContactEmail, string? ContactPhone,
    string CompanyName, string? TradeName, string Cnpj,
    ClientSegment? Segment, CompanySize? SizeCategory,
    string? City, string? State, string? ReferralCode);

public record SubmitPartnerRequest(
    string ContactName, string ContactEmail, string? ContactPhone,
    string CompanyName, string? TradeName, string Cnpj,
    ClientSegment? Segment, CompanySize? SizeCategory,
    string? City, string? State);

public record ReviewRequest(string? Notes);
