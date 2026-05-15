using Gdac.Core.Application.Features.Companies.Commands.AddUserToCompany;
using Gdac.Core.Application.Features.Companies.Commands.CreateCompany;
using Gdac.Core.Application.Features.Companies.Commands.DeactivateCompany;
using Gdac.Core.Application.Features.Companies.Commands.RemoveUserFromCompany;
using Gdac.Core.Application.Features.Companies.Commands.SyncCompanyCnpjData;
using Gdac.Core.Application.Features.Companies.Commands.SyncCompanyMembers;
using Gdac.Core.Application.Features.Companies.Commands.SyncCompanyOffices;
using Gdac.Core.Application.Features.Companies.Commands.UpdateCompany;
using Gdac.Core.Application.Features.Companies.Queries.GetCompanies;
using Gdac.Core.Application.Features.Companies.Queries.GetCompany;
using Gdac.Core.Application.Features.Companies.Queries.GetCompanyMembers;
using Gdac.Core.Application.Features.Companies.Queries.GetCompanyOffices;
using Gdac.Core.Application.Features.Companies.Queries.GetCompanyUsers;
using Gdac.Core.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Core.Api.Controllers;

[ApiController]
[Route("companies")]
[Authorize]
public class CompanyController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetCompaniesQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetCompanyQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateCompanyCommand(
            request.Name, request.Type, request.TradeName,
            request.Cnpj, request.Email, request.Phone,
            request.Segment, request.SizeCategory), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCompanyCommand(
            id, request.Name, request.TradeName, request.Cnpj,
            request.Type, request.Email, request.Phone,
            request.Segment, request.SizeCategory), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeactivateCompanyCommand(id), ct);
        return NoContent();
    }

    // CNPJ API data sync

    [HttpPut("{id:guid}/cnpj-data")]
    public async Task<IActionResult> SyncCnpjData(Guid id, [FromBody] SyncCnpjDataRequest request, CancellationToken ct)
    {
        await mediator.Send(new SyncCompanyCnpjDataCommand(
            id, request.CnpjBase,
            request.NatureId, request.NatureText,
            request.SizeId, request.SizeAcronym, request.SizeText,
            request.Equity, request.Jurisdiction,
            request.SimplesOptant, request.SimplesSince,
            request.SimeiOptant, request.SimeiSince), ct);
        return NoContent();
    }

    // Members (sócios)

    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetCompanyMembersQuery(id), ct));

    [HttpPut("{id:guid}/members")]
    public async Task<IActionResult> SyncMembers(Guid id, [FromBody] SyncMembersRequest request, CancellationToken ct)
    {
        await mediator.Send(new SyncCompanyMembersCommand(id, request.Members
            .Select(m => new MemberInput(
                m.PersonType, m.PersonName, m.RoleId, m.RoleText,
                m.Since, m.PersonExternalId, m.PersonTaxId, m.PersonAge,
                m.AgentName, m.AgentTaxId, m.AgentRoleId, m.AgentRoleText))
            .ToList()), ct);
        return NoContent();
    }

    // Offices (estabelecimentos)

    [HttpGet("{id:guid}/offices")]
    public async Task<IActionResult> GetOffices(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetCompanyOfficesQuery(id), ct));

    [HttpPut("{id:guid}/offices")]
    public async Task<IActionResult> SyncOffices(Guid id, [FromBody] SyncOfficesRequest request, CancellationToken ct)
    {
        await mediator.Send(new SyncCompanyOfficesCommand(id, request.Offices
            .Select(o => new OfficeInput(
                o.TaxId, o.StatusId, o.StatusText,
                o.IsHead, o.Alias, o.Founded, o.StatusDate,
                o.ReasonId, o.ReasonText, o.MainActivityId, o.MainActivityText))
            .ToList()), ct);
        return NoContent();
    }

    // Users (vínculos internos)

    [HttpGet("{id:guid}/users")]
    public async Task<IActionResult> GetUsers(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetCompanyUsersQuery(id), ct));

    [HttpPost("{id:guid}/users")]
    public async Task<IActionResult> AddUser(Guid id, [FromBody] AddUserRequest request, CancellationToken ct)
    {
        await mediator.Send(new AddUserToCompanyCommand(id, request.UserId, request.Role), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/users/{userId:guid}")]
    public async Task<IActionResult> RemoveUser(Guid id, Guid userId, CancellationToken ct)
    {
        await mediator.Send(new RemoveUserFromCompanyCommand(id, userId), ct);
        return NoContent();
    }
}

public record CreateCompanyRequest(
    string Name, CompanyType Type, string? TradeName, string? Cnpj,
    string? Email, string? Phone, ClientSegment? Segment, CompanySize? SizeCategory);

public record UpdateCompanyRequest(
    string Name, string? TradeName, string? Cnpj, CompanyType Type,
    string? Email, string? Phone, ClientSegment? Segment, CompanySize? SizeCategory);

public record SyncCnpjDataRequest(
    string? CnpjBase,
    int? NatureId, string? NatureText,
    int? SizeId, string? SizeAcronym, string? SizeText,
    decimal? Equity, string? Jurisdiction,
    bool SimplesOptant, DateOnly? SimplesSince,
    bool SimeiOptant, DateOnly? SimeiSince);

public record SyncMembersRequest(IReadOnlyList<MemberInputRequest> Members);
public record MemberInputRequest(
    PersonType PersonType, string PersonName, int RoleId, string RoleText,
    DateOnly? Since, string? PersonExternalId, string? PersonTaxId, string? PersonAge,
    string? AgentName, string? AgentTaxId, int? AgentRoleId, string? AgentRoleText);

public record SyncOfficesRequest(IReadOnlyList<OfficeInputRequest> Offices);
public record OfficeInputRequest(
    string TaxId, int StatusId, string StatusText,
    bool IsHead, string? Alias, DateOnly? Founded, DateOnly? StatusDate,
    int? ReasonId, string? ReasonText, int? MainActivityId, string? MainActivityText);

public record AddUserRequest(Guid UserId, string Role);
