using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetCompanyById;

public record GetCompanyByIdQuery(Guid CompanyId) : IRequest<GetCompanyByIdResult>;
public record GetCompanyByIdResult(Guid Id, string ExternalId, string Name, bool IsActive);
