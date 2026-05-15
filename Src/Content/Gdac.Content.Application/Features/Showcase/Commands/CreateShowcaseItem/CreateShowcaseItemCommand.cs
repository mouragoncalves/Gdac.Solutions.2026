using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Commands.CreateShowcaseItem;

public record CreateShowcaseItemCommand(
    ShowcaseItemType Type,
    Guid             CoreCompanyId,
    string           Name,
    string?          LogoUrl) : IRequest<Guid>;
