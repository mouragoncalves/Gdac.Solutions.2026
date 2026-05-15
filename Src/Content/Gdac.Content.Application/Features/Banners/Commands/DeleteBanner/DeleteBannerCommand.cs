using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.DeleteBanner;

public record DeleteBannerCommand(Guid Id) : IRequest;
