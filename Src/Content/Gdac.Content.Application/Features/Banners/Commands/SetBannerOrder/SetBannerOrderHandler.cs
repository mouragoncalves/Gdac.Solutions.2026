using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.SetBannerOrder;

public class SetBannerOrderHandler(IBannerRepository repo, IUnitOfWork uow)
    : IRequestHandler<SetBannerOrderCommand>
{
    public async Task Handle(SetBannerOrderCommand request, CancellationToken ct)
    {
        var banner = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Banner), request.Id);

        banner.SetOrder(request.Order);
        repo.Update(banner);
        await uow.CommitAsync(ct);
    }
}
