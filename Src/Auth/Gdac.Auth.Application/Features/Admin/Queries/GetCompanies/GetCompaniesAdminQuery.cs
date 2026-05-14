using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetCompanies;

public record GetCompaniesAdminQuery : IRequest<GetCompaniesAdminResult>;
public record CompanySummaryDto(Guid Id, string ExternalId, string Name, bool IsActive);
public record GetCompaniesAdminResult(IReadOnlyList<CompanySummaryDto> Items);
