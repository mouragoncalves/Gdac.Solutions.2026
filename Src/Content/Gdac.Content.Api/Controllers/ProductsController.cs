using Gdac.Content.Application.Features.Products.Commands.AddProductMedia;
using Gdac.Content.Application.Features.Products.Commands.CreateProduct;
using Gdac.Content.Application.Features.Products.Commands.DeleteProduct;
using Gdac.Content.Application.Features.Products.Commands.DeleteProductMedia;
using Gdac.Content.Application.Features.Products.Commands.SetProductActive;
using Gdac.Content.Application.Features.Products.Commands.UpdateProductInfo;
using Gdac.Content.Application.Features.Products.Commands.UpdateProductPricing;
using Gdac.Content.Application.Features.Products.Queries.GetProduct;
using Gdac.Content.Application.Features.Products.Queries.GetProductPriceHistory;
using Gdac.Content.Application.Features.Products.Queries.GetProducts;
using Gdac.Content.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Content.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetProductsQuery(), ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProductQuery(id), ct));

    [HttpGet("{id:guid}/price-history")]
    [Authorize]
    public async Task<IActionResult> GetPriceHistory(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProductPriceHistoryQuery(id), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateProductCommand(
            request.Name, request.Category, request.Description,
            request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}/info")]
    [Authorize]
    public async Task<IActionResult> UpdateInfo(Guid id, [FromBody] UpdateProductInfoRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateProductInfoCommand(
            id, request.Name, request.Category, request.Description,
            request.IsFeatured, request.DisplayOrder), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/pricing")]
    [Authorize]
    public async Task<IActionResult> UpdatePricing(Guid id, [FromBody] UpdateProductPricingRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateProductPricingCommand(
            id, request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual,
            request.ChangedBy, request.Notes), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetActiveRequest request, CancellationToken ct)
    {
        await mediator.Send(new SetProductActiveCommand(id, request.IsActive), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/media")]
    [Authorize]
    public async Task<IActionResult> AddMedia(Guid id, [FromBody] AddMediaRequest request, CancellationToken ct)
    {
        var mediaId = await mediator.Send(new AddProductMediaCommand(
            id, request.Url, request.Type, request.DisplayOrder), ct);
        return Created($"/products/{id}/media/{mediaId}", new { id = mediaId });
    }

    [HttpDelete("{id:guid}/media/{mediaId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteMedia(Guid id, Guid mediaId, CancellationToken ct)
    {
        await mediator.Send(new DeleteProductMediaCommand(id, mediaId), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}

public record CreateProductRequest(
    string  Name,
    string  Category,
    string  Description,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral = 10m,
    decimal DescontoAnual     = 25m);

public record UpdateProductInfoRequest(
    string Name,
    string Category,
    string Description,
    bool   IsFeatured,
    int    DisplayOrder);

public record UpdateProductPricingRequest(
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral,
    decimal DescontoAnual,
    Guid    ChangedBy,
    string? Notes);

public record AddMediaRequest(string Url, MediaType Type, int DisplayOrder);
