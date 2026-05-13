using MediatR;

namespace Gdac.Auth.Application.Features.Session.Queries.GetCompanies;

public record GetCompaniesQuery(Guid UserId) : IRequest<GetCompaniesResult>;

public record CompanyDto(Guid Id, string ExternalId, string Name);

public record GetCompaniesResult(IReadOnlyList<CompanyDto> Companies);
