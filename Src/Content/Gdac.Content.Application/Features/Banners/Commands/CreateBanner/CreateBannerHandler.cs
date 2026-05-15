using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.CreateBanner;

public class CreateBannerHandler(IBannerRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateBannerCommand, Guid>
{
    public async Task<Guid> Handle(CreateBannerCommand request, CancellationToken ct)
    {
        var banner = Banner.Create(
            request.Title, request.Subtitle,
            request.ImageUrl, request.CtaText, request.CtaUrl,
            request.SecondaryCtaText, request.SecondaryCtaUrl,
            request.PartnerId);

        await repo.AddAsync(banner, ct);
        await uow.CommitAsync(ct);
        return banner.Id;
    }
}
