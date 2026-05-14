using Gdac.Auth.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Auth.Api.Controllers;

[ApiController]
public class WellKnownController(ITokenService tokenService) : ControllerBase
{
    [HttpGet(".well-known/jwks.json")]
    [ResponseCache(Duration = 3600)]
    public IActionResult Jwks()
    {
        var json = tokenService.GetPublicKeyJwks();
        return Content(json, "application/json");
    }
}
