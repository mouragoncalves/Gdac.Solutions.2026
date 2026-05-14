using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanyUsers;

public class GetCompanyUsersHandler(ICompanyRepository companyRepo, IUserCompanyLinkRepository linkRepo)
    : IRequestHandler<GetCompanyUsersQuery, IReadOnlyList<CompanyUserResult>>
{
    public async Task<IReadOnlyList<CompanyUserResult>> Handle(GetCompanyUsersQuery request, CancellationToken ct)
    {
        var company = await companyRepo.GetByIdWithUsersAsync(request.CompanyId, ct)
            ?? throw new NotFoundException("Empresa", request.CompanyId);

        var links = await linkRepo.GetByCompanyAsync(request.CompanyId, ct);
        return links.Select(l => new CompanyUserResult(
            l.UserId, l.User.FullName, l.User.Email, l.Role, l.IsActive, l.JoinedAt)).ToList();
    }
}
