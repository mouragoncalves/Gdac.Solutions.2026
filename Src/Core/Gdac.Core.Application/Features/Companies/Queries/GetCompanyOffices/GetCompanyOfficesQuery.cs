using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanyOffices;

public record GetCompanyOfficesQuery(Guid CompanyId) : IRequest<IReadOnlyList<CompanyOfficeResult>>;

public record CompanyOfficeResult(
    Guid Id,
    string TaxId,
    string? Alias,
    DateOnly? Founded,
    bool IsHead,
    int StatusId,
    string StatusText,
    DateOnly? StatusDate,
    int? ReasonId,
    string? ReasonText,
    int? MainActivityId,
    string? MainActivityText);
