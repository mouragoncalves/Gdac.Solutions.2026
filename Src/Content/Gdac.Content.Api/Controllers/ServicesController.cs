using Gdac.Content.Application.Features.Services.Commands.AddServiceMedia;
using Gdac.Content.Application.Features.Services.Commands.CreateContentService;
using Gdac.Content.Application.Features.Services.Commands.DeleteContentService;
using Gdac.Content.Application.Features.Services.Commands.DeleteServiceMedia;
using Gdac.Content.Application.Features.Services.Commands.SetContentServiceActive;
using Gdac.Content.Application.Features.Services.Commands.UpdateContentServiceInfo;
using Gdac.Content.Application.Features.Services.Commands.UpdateContentServicePricing;
using Gdac.Content.Application.Features.Services.Queries.GetContentService;
using Gdac.Content.Application.Features.Services.Queries.GetContentServicePriceHistory;
using Gdac.Content.Application.Features.Services.Queries.GetContentServices;
using Gdac.Content.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Content.Api.Controllers;

[ApiController]
[Route("services")]
public class ServicesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetContentServicesQuery(), ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetContentServiceQuery(id), ct));

    [HttpGet("{id:guid}/price-history")]
    [Authorize]
    public async Task<IActionResult> GetPriceHistory(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetContentServicePriceHistoryQuery(id), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateContentServiceCommand(
            request.Name, request.Category, request.Description,
            request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}/info")]
    [Authorize]
    public async Task<IActionResult> UpdateInfo(Guid id, [FromBody] UpdateServiceInfoRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateContentServiceInfoCommand(
            id, request.Name, request.Category, request.Description,
            request.IsFeatured, request.DisplayOrder), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/pricing")]
    [Authorize]
    public async Task<IActionResult> UpdatePricing(Guid id, [FromBody] UpdateServicePricingRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateContentServicePricingCommand(
            id, request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual,
            request.ChangedBy, request.Notes), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetActiveRequest request, CancellationToken ct)
    {
        await mediator.Send(new SetContentServiceActiveCommand(id, request.IsActive), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/media")]
    [Authorize]
    public async Task<IActionResult> AddMedia(Guid id, [FromBody] AddMediaRequest request, CancellationToken ct)
    {
        var mediaId = await mediator.Send(new AddServiceMediaCommand(
            id, request.Url, request.Type, request.DisplayOrder), ct);
        return Created($"/services/{id}/media/{mediaId}", new { id = mediaId });
    }

    [HttpDelete("{id:guid}/media/{mediaId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteMedia(Guid id, Guid mediaId, CancellationToken ct)
    {
        await mediator.Send(new DeleteServiceMediaCommand(id, mediaId), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteContentServiceCommand(id), ct);
        return NoContent();
    }
}

public record CreateServiceRequest(
    string  Name,
    string  Category,
    string  Description,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral = 10m,
    decimal DescontoAnual     = 25m);

public record UpdateServiceInfoRequest(
    string Name,
    string Category,
    string Description,
    bool   IsFeatured,
    int    DisplayOrder);

public record UpdateServicePricingRequest(
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral,
    decimal DescontoAnual,
    Guid    ChangedBy,
    string? Notes);
