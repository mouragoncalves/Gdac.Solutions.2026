using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.AddServiceMedia;

public record AddServiceMediaCommand(
    Guid      ServiceId,
    string    Url,
    MediaType Type,
    int       DisplayOrder) : IRequest<Guid>;
