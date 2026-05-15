using Gdac.Content.Application.Features.Showcase.Commands.CreateShowcaseItem;
using Gdac.Content.Application.Features.Showcase.Commands.DeleteShowcaseItem;
using Gdac.Content.Application.Features.Showcase.Commands.SetShowcaseItemActive;
using Gdac.Content.Application.Features.Showcase.Commands.UpdateShowcaseItem;
using Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItem;
using Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItems;
using Gdac.Content.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Content.Api.Controllers;

[ApiController]
[Route("showcase")]
public class ShowcaseController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ShowcaseItemType? type, CancellationToken ct) =>
        Ok(await mediator.Send(new GetShowcaseItemsQuery(type), ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetShowcaseItemQuery(id), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateShowcaseItemRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateShowcaseItemCommand(
            request.Type, request.CoreCompanyId, request.Name, request.LogoUrl), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShowcaseItemRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateShowcaseItemCommand(
            id, request.Type, request.CoreCompanyId, request.Name, request.LogoUrl), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetActiveRequest request, CancellationToken ct)
    {
        await mediator.Send(new SetShowcaseItemActiveCommand(id, request.IsActive), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteShowcaseItemCommand(id), ct);
        return NoContent();
    }
}

public record CreateShowcaseItemRequest(
    ShowcaseItemType Type,
    Guid             CoreCompanyId,
    string           Name,
    string?          LogoUrl);

public record UpdateShowcaseItemRequest(
    ShowcaseItemType Type,
    Guid             CoreCompanyId,
    string           Name,
    string?          LogoUrl);
