using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.RemoveUserFromCompany;

public class RemoveUserFromCompanyHandler(IUserCompanyLinkRepository linkRepo, IUnitOfWork uow)
    : IRequestHandler<RemoveUserFromCompanyCommand>
{
    public async Task Handle(RemoveUserFromCompanyCommand request, CancellationToken ct)
    {
        var link = await linkRepo.GetAsync(request.UserId, request.CompanyId, ct)
            ?? throw new NotFoundException("Vínculo", $"{request.UserId}/{request.CompanyId}");

        link.Deactivate();
        linkRepo.Update(link);
        await uow.CommitAsync(ct);
    }
}
