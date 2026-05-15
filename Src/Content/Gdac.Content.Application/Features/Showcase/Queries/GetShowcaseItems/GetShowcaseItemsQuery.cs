using Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItem;
using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItems;

public record GetShowcaseItemsQuery(ShowcaseItemType? Type = null) : IRequest<IReadOnlyList<ShowcaseItemResult>>;
