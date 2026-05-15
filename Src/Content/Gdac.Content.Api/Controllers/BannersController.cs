using Gdac.Content.Application.Features.Banners.Commands.CreateBanner;
using Gdac.Content.Application.Features.Banners.Commands.DeleteBanner;
using Gdac.Content.Application.Features.Banners.Commands.SetBannerActive;
using Gdac.Content.Application.Features.Banners.Commands.SetBannerOrder;
using Gdac.Content.Application.Features.Banners.Commands.UpdateBanner;
using Gdac.Content.Application.Features.Banners.Queries.GetBanner;
using Gdac.Content.Application.Features.Banners.Queries.GetBanners;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Content.Api.Controllers;

[ApiController]
[Route("banners")]
public class BannersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] Guid? partnerId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetBannersQuery(partnerId), ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetBannerQuery(id), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateBannerRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateBannerCommand(
            request.Title, request.Subtitle, request.ImageUrl,
            request.CtaText, request.CtaUrl,
            request.SecondaryCtaText, request.SecondaryCtaUrl,
            request.PartnerId), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBannerRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateBannerCommand(
            id, request.Title, request.Subtitle, request.ImageUrl,
            request.CtaText, request.CtaUrl,
            request.SecondaryCtaText, request.SecondaryCtaUrl), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/order")]
    [Authorize]
    public async Task<IActionResult> SetOrder(Guid id, [FromBody] SetOrderRequest request, CancellationToken ct)
    {
        await mediator.Send(new SetBannerOrderCommand(id, request.Order), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetActiveRequest request, CancellationToken ct)
    {
        await mediator.Send(new SetBannerActiveCommand(id, request.IsActive), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteBannerCommand(id), ct);
        return NoContent();
    }
}

public record CreateBannerRequest(
    string  Title,
    string? Subtitle,
    string  ImageUrl,
    string  CtaText,
    string  CtaUrl,
    string? SecondaryCtaText,
    string? SecondaryCtaUrl,
    Guid?   PartnerId);

public record UpdateBannerRequest(
    string  Title,
    string? Subtitle,
    string  ImageUrl,
    string  CtaText,
    string  CtaUrl,
    string? SecondaryCtaText,
    string? SecondaryCtaUrl);

public record SetOrderRequest(int Order);
public record SetActiveRequest(bool IsActive);
