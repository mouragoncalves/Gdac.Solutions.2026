using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.SetBannerActive;

public class SetBannerActiveHandler(IBannerRepository repo, IUnitOfWork uow)
    : IRequestHandler<SetBannerActiveCommand>
{
    public async Task Handle(SetBannerActiveCommand request, CancellationToken ct)
    {
        var banner = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Banner), request.Id);

        banner.SetActive(request.IsActive);
        repo.Update(banner);
        await uow.CommitAsync(ct);
    }
}
