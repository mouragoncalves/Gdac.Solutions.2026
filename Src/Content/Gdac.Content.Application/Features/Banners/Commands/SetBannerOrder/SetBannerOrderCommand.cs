using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.SetBannerOrder;

public record SetBannerOrderCommand(Guid Id, int Order) : IRequest;
