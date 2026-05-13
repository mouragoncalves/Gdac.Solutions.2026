using Gdac.Auth.Application.Features.Demo.Commands.RegisterDemo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/demo")]
public class DemoController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDemoRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterDemoCommand(request.Name, request.Email), ct);
        return Ok(result);
    }
}

public record RegisterDemoRequest(string Name, string Email);
