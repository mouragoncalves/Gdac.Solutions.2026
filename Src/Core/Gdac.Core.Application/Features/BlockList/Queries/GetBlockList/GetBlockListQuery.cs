using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Queries.GetBlockList;

public record GetBlockListQuery : IRequest<IReadOnlyList<BlockRecordResult>>;

public record BlockRecordResult(
    Guid Id,
    string CnpjBase,
    string? Reason,
    Guid BlockedBy,
    DateTime CreatedAt);
