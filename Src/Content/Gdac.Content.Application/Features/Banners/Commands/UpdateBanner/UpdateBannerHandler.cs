using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.UpdateBanner;

public class UpdateBannerHandler(IBannerRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateBannerCommand>
{
    public async Task Handle(UpdateBannerCommand request, CancellationToken ct)
    {
        var banner = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Banner), request.Id);

        banner.Update(
            request.Title, request.Subtitle,
            request.ImageUrl, request.CtaText, request.CtaUrl,
            request.SecondaryCtaText, request.SecondaryCtaUrl);

        repo.Update(banner);
        await uow.CommitAsync(ct);
    }
}
