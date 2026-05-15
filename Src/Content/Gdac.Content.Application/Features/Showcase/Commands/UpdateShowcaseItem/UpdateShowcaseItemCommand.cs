using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Commands.UpdateShowcaseItem;

public record UpdateShowcaseItemCommand(
    Guid             Id,
    ShowcaseItemType Type,
    Guid             CoreCompanyId,
    string           Name,
    string?          LogoUrl) : IRequest;
