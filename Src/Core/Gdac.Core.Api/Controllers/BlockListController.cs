using Gdac.Core.Application.Features.BlockList.Commands.BlockCnpj;
using Gdac.Core.Application.Features.BlockList.Commands.UnblockCnpj;
using Gdac.Core.Application.Features.BlockList.Queries.CheckCnpjBlocked;
using Gdac.Core.Application.Features.BlockList.Queries.GetBlockList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gdac.Core.Api.Controllers;

[ApiController]
[Route("block-list")]
[Authorize]
public class BlockListController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<BlockRecordResult>> GetAll(CancellationToken ct)
        => await mediator.Send(new GetBlockListQuery(), ct);

    [HttpGet("{cnpjBase}")]
    public async Task<bool> Check([FromRoute] string cnpjBase, CancellationToken ct)
        => await mediator.Send(new CheckCnpjBlockedQuery(cnpjBase), ct);

    [HttpPost]
    public async Task<IActionResult> Block([FromBody] BlockRequest request, CancellationToken ct)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var blockedBy = Guid.TryParse(userId, out var id) ? id : Guid.Empty;

        await mediator.Send(new BlockCnpjCommand(request.CnpjBase, blockedBy, request.Reason), ct);
        return NoContent();
    }

    [HttpDelete("{cnpjBase}")]
    public async Task<IActionResult> Unblock([FromRoute] string cnpjBase, CancellationToken ct)
    {
        await mediator.Send(new UnblockCnpjCommand(cnpjBase), ct);
        return NoContent();
    }
}

public record BlockRequest(string CnpjBase, string? Reason);
