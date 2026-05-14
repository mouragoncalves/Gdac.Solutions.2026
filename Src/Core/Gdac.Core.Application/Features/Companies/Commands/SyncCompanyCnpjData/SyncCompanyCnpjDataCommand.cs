using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.SyncCompanyCnpjData;

public record SyncCompanyCnpjDataCommand(
    Guid CompanyId,
    string? CnpjBase,
    int? NatureId,
    string? NatureText,
    int? SizeId,
    string? SizeAcronym,
    string? SizeText,
    decimal? Equity,
    string? Jurisdiction,
    bool SimplesOptant,
    DateOnly? SimplesSince,
    bool SimeiOptant,
    DateOnly? SimeiSince) : IRequest;
