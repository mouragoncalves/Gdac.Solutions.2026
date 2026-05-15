using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.DeleteBanner;

public class DeleteBannerHandler(IBannerRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteBannerCommand>
{
    public async Task Handle(DeleteBannerCommand request, CancellationToken ct)
    {
        var banner = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Banner), request.Id);

        banner.SetActive(false);
        repo.Update(banner);
        await uow.CommitAsync(ct);
    }
}
