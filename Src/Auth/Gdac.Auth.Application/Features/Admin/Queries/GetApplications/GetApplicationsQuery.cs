using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetApplications;

public record GetApplicationsQuery : IRequest<GetApplicationsResult>;
public record ApplicationSummaryDto(Guid Id, string Name, string ClientId, bool IsActive, DateTime CreatedAt);
public record GetApplicationsResult(IReadOnlyList<ApplicationSummaryDto> Items);
