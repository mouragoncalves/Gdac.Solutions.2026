using Gdac.Content.Application.Features.Testimonials.Commands.CreateTestimonial;
using Gdac.Content.Application.Features.Testimonials.Commands.DeleteTestimonial;
using Gdac.Content.Application.Features.Testimonials.Commands.SetTestimonialActive;
using Gdac.Content.Application.Features.Testimonials.Commands.UpdateTestimonial;
using Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonial;
using Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonials;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Content.Api.Controllers;

[ApiController]
[Route("testimonials")]
public class TestimonialsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] Guid? partnerId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetTestimonialsQuery(partnerId), ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetTestimonialQuery(id), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateTestimonialRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateTestimonialCommand(
            request.AuthorName, request.AuthorRole, request.AuthorCompany,
            request.AuthorPhotoUrl, request.Content, request.Rating, request.PartnerId), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestimonialRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateTestimonialCommand(
            id, request.AuthorName, request.AuthorRole, request.AuthorCompany,
            request.AuthorPhotoUrl, request.Content, request.Rating), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetActiveRequest request, CancellationToken ct)
    {
        await mediator.Send(new SetTestimonialActiveCommand(id, request.IsActive), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteTestimonialCommand(id), ct);
        return NoContent();
    }
}

public record CreateTestimonialRequest(
    string  AuthorName,
    string? AuthorRole,
    string? AuthorCompany,
    string? AuthorPhotoUrl,
    string  Content,
    int     Rating,
    Guid?   PartnerId);

public record UpdateTestimonialRequest(
    string  AuthorName,
    string? AuthorRole,
    string? AuthorCompany,
    string? AuthorPhotoUrl,
    string  Content,
    int     Rating);
