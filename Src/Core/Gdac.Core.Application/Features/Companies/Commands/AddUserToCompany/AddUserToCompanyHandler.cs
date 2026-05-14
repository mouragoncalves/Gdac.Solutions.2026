using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.AddUserToCompany;

public class AddUserToCompanyHandler(
    ICompanyRepository companyRepo,
    IUserProfileRepository userRepo,
    IUserCompanyLinkRepository linkRepo,
    IUnitOfWork uow)
    : IRequestHandler<AddUserToCompanyCommand>
{
    public async Task Handle(AddUserToCompanyCommand request, CancellationToken ct)
    {
        if (!await companyRepo.ExistsByCnpjAsync(string.Empty, ct) &&
            await companyRepo.GetByIdAsync(request.CompanyId, ct) is null)
            throw new NotFoundException("Empresa", request.CompanyId);

        if (!await userRepo.ExistsAsync(request.UserId, ct))
            throw new NotFoundException("Perfil", request.UserId);

        var existing = await linkRepo.GetAsync(request.UserId, request.CompanyId, ct);
        if (existing is not null)
        {
            if (existing.IsActive)
                throw new DomainException("Usuário já vinculado a esta empresa.");
            existing.UpdateRole(request.Role);
            linkRepo.Update(existing);
        }
        else
        {
            var link = UserCompanyLink.Create(request.UserId, request.CompanyId, request.Role);
            await linkRepo.AddAsync(link, ct);
        }

        await uow.CommitAsync(ct);
    }
}
