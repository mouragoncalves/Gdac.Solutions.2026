using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.UpdateContentServiceInfo;

public record UpdateContentServiceInfoCommand(
    Guid   Id,
    string Name,
    string Category,
    string Description,
    bool   IsFeatured,
    int    DisplayOrder) : IRequest;
