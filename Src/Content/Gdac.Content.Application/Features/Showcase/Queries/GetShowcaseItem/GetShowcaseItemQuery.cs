using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItem;

public record GetShowcaseItemQuery(Guid Id) : IRequest<ShowcaseItemResult>;

public record ShowcaseItemResult(
    Guid             Id,
    ShowcaseItemType Type,
    Guid             CoreCompanyId,
    string           Name,
    string?          LogoUrl,
    bool             IsActive,
    int              DisplayOrder,
    DateTime         CreatedAt,
    DateTime         UpdatedAt);
