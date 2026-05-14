using Gdac.Core.Application.Features.UserProfiles.Commands.CreateUserProfile;
using Gdac.Core.Application.Features.UserProfiles.Commands.UpdateUserProfile;
using Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfile;
using Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Core.Api.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserProfilesQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserProfileQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserProfileCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserProfileRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateUserProfileCommand(
            id, request.FullName, request.Phone, request.AvatarUrl, request.Cpf, request.BirthDate), ct);
        return NoContent();
    }
}

public record UpdateUserProfileRequest(
    string FullName, string? Phone, string? AvatarUrl, string? Cpf, DateOnly? BirthDate);
