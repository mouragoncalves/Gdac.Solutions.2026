using Gdac.Auth.Application.Features.Admin.Commands.AssignUserApplication;
using Gdac.Auth.Application.Features.Admin.Commands.AssignUserCompany;
using Gdac.Auth.Application.Features.Admin.Commands.CreateApplication;
using Gdac.Auth.Application.Features.Admin.Commands.CreateCompany;
using Gdac.Auth.Application.Features.Admin.Commands.CreateUser;
using Gdac.Auth.Application.Features.Admin.Commands.DeactivateUser;
using Gdac.Auth.Application.Features.Admin.Commands.RemoveUserApplication;
using Gdac.Auth.Application.Features.Admin.Commands.RemoveUserCompany;
using Gdac.Auth.Application.Features.Admin.Commands.RotateApplicationSecret;
using Gdac.Auth.Application.Features.Admin.Commands.UpdateApplication;
using Gdac.Auth.Application.Features.Admin.Commands.UpdateCompany;
using Gdac.Auth.Application.Features.Admin.Commands.UpdateUser;
using Gdac.Auth.Application.Features.Admin.Queries.GetApplicationById;
using Gdac.Auth.Application.Features.Admin.Queries.GetApplications;
using Gdac.Auth.Application.Features.Admin.Queries.GetCompanies;
using Gdac.Auth.Application.Features.Admin.Queries.GetCompanyById;
using Gdac.Auth.Application.Features.Admin.Queries.GetUserById;
using Gdac.Auth.Application.Features.Admin.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminController(IMediator mediator) : ControllerBase
{
    // ── Usuários ──────────────────────────────────────────────────────────────

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await mediator.Send(new GetUsersQuery(page, pageSize), ct));

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetUserByIdQuery(id), ct));

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateUserCommand(req.Email, req.MustChangePassword), ct);
        return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
    }

    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest req, CancellationToken ct)
    {
        await mediator.Send(new UpdateUserCommand(id, req.IsActive, req.MustChangePassword), ct);
        return NoContent();
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeactivateUserCommand(id), ct);
        return NoContent();
    }

    [HttpPost("users/{userId:guid}/applications")]
    public async Task<IActionResult> AssignApplication(Guid userId, [FromBody] AssignApplicationRequest req, CancellationToken ct)
    {
        await mediator.Send(new AssignUserApplicationCommand(userId, req.ApplicationId), ct);
        return NoContent();
    }

    [HttpDelete("users/{userId:guid}/applications/{applicationId:guid}")]
    public async Task<IActionResult> RemoveApplication(Guid userId, Guid applicationId, CancellationToken ct)
    {
        await mediator.Send(new RemoveUserApplicationCommand(userId, applicationId), ct);
        return NoContent();
    }

    [HttpPost("users/{userId:guid}/companies")]
    public async Task<IActionResult> AssignCompany(Guid userId, [FromBody] AssignCompanyRequest req, CancellationToken ct)
    {
        await mediator.Send(new AssignUserCompanyCommand(userId, req.CompanyId), ct);
        return NoContent();
    }

    [HttpDelete("users/{userId:guid}/companies/{companyId:guid}")]
    public async Task<IActionResult> RemoveCompany(Guid userId, Guid companyId, CancellationToken ct)
    {
        await mediator.Send(new RemoveUserCompanyCommand(userId, companyId), ct);
        return NoContent();
    }

    // ── Aplicações ────────────────────────────────────────────────────────────

    [HttpGet("applications")]
    public async Task<IActionResult> GetApplications(CancellationToken ct)
        => Ok(await mediator.Send(new GetApplicationsQuery(), ct));

    [HttpGet("applications/{id:guid}")]
    public async Task<IActionResult> GetApplication(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetApplicationByIdQuery(id), ct));

    [HttpPost("applications")]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateApplicationCommand(req.Name, req.ClientId), ct);
        return CreatedAtAction(nameof(GetApplication), new { id = result.Id }, result);
    }

    [HttpPut("applications/{id:guid}")]
    public async Task<IActionResult> UpdateApplication(Guid id, [FromBody] UpdateApplicationRequest req, CancellationToken ct)
    {
        await mediator.Send(new UpdateApplicationCommand(id, req.Name, req.IsActive), ct);
        return NoContent();
    }

    [HttpPost("applications/{id:guid}/rotate-secret")]
    public async Task<IActionResult> RotateSecret(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new RotateApplicationSecretCommand(id), ct));

    // ── Empresas ──────────────────────────────────────────────────────────────

    [HttpGet("companies")]
    public async Task<IActionResult> GetCompanies(CancellationToken ct)
        => Ok(await mediator.Send(new GetCompaniesAdminQuery(), ct));

    [HttpGet("companies/{id:guid}")]
    public async Task<IActionResult> GetCompany(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetCompanyByIdQuery(id), ct));

    [HttpPost("companies")]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateCompanyCommand(req.ExternalId, req.Name), ct);
        return CreatedAtAction(nameof(GetCompany), new { id = result.Id }, result);
    }

    [HttpPut("companies/{id:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequest req, CancellationToken ct)
    {
        await mediator.Send(new UpdateCompanyCommand(id, req.Name, req.IsActive), ct);
        return NoContent();
    }
}

// ── DTOs de request ──────────────────────────────────────────────────────────

public record CreateUserRequest(string Email, bool MustChangePassword = true);
public record UpdateUserRequest(bool IsActive, bool MustChangePassword);
public record AssignApplicationRequest(Guid ApplicationId);
public record AssignCompanyRequest(Guid CompanyId);
public record CreateApplicationRequest(string Name, string ClientId);
public record UpdateApplicationRequest(string Name, bool IsActive);
public record CreateCompanyRequest(string ExternalId, string Name);
public record UpdateCompanyRequest(string Name, bool IsActive);
