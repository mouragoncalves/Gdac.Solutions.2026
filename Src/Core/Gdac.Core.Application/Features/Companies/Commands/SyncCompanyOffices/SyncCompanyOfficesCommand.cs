using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.SyncCompanyOffices;

public record SyncCompanyOfficesCommand(
    Guid CompanyId,
    IReadOnlyList<OfficeInput> Offices) : IRequest;

public record OfficeInput(
    string TaxId,
    int StatusId,
    string StatusText,
    bool IsHead = false,
    string? Alias = null,
    DateOnly? Founded = null,
    DateOnly? StatusDate = null,
    int? ReasonId = null,
    string? ReasonText = null,
    int? MainActivityId = null,
    string? MainActivityText = null);
