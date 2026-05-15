using Gdac.Content.Application.Features.Integrations.Commands.CreateIntegration;
using Gdac.Content.Application.Features.Integrations.Commands.DeleteIntegration;
using Gdac.Content.Application.Features.Integrations.Commands.SetIntegrationActive;
using Gdac.Content.Application.Features.Integrations.Commands.UpdateIntegration;
using Gdac.Content.Application.Features.Integrations.Queries.GetIntegration;
using Gdac.Content.Application.Features.Integrations.Queries.GetIntegrations;
using Gdac.Content.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Content.Api.Controllers;

[ApiController]
[Route("integrations")]
public class IntegrationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] IntegrationCategory? category, CancellationToken ct) =>
        Ok(await mediator.Send(new GetIntegrationsQuery(category), ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetIntegrationQuery(id), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateIntegrationRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateIntegrationCommand(
            request.Name, request.Category, request.LogoUrl,
            request.Description, request.ExternalUrl), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIntegrationRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateIntegrationCommand(
            id, request.Name, request.Category, request.LogoUrl,
            request.Description, request.ExternalUrl), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetActiveRequest request, CancellationToken ct)
    {
        await mediator.Send(new SetIntegrationActiveCommand(id, request.IsActive), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteIntegrationCommand(id), ct);
        return NoContent();
    }
}

public record CreateIntegrationRequest(
    string              Name,
    IntegrationCategory Category,
    string              LogoUrl,
    string              Description,
    string?             ExternalUrl);

public record UpdateIntegrationRequest(
    string              Name,
    IntegrationCategory Category,
    string              LogoUrl,
    string              Description,
    string?             ExternalUrl);
