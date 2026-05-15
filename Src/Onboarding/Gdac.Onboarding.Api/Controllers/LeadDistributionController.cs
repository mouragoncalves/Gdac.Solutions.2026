using Gdac.Onboarding.Application.Features.LeadDistribution.Commands.UpdateLeadDistributionConfig;
using Gdac.Onboarding.Application.Features.LeadDistribution.Queries.GetLeadDistributionConfig;
using Gdac.Onboarding.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Onboarding.Api.Controllers;

[ApiController]
[Route("lead-distribution-config")]
[Authorize]
public class LeadDistributionController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await mediator.Send(new GetLeadDistributionConfigQuery(), ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateLeadDistributionConfigRequest request, CancellationToken ct)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var updatedBy = Guid.TryParse(userId, out var id) ? id : Guid.Empty;

        await mediator.Send(new UpdateLeadDistributionConfigCommand(
            request.Mode, request.DefaultPartnerId, updatedBy), ct);
        return NoContent();
    }
}

public record UpdateLeadDistributionConfigRequest(LeadDistributionMode Mode, Guid? DefaultPartnerId);
