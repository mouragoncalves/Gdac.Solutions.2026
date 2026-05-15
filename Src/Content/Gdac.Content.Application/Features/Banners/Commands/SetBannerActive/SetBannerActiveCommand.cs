using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.SetBannerActive;

public record SetBannerActiveCommand(Guid Id, bool IsActive) : IRequest;
