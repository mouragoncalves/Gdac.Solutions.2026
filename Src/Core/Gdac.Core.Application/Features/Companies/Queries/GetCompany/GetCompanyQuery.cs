using Gdac.Core.Domain.Enums;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompany;

public record GetCompanyQuery(Guid Id) : IRequest<CompanyResult>;

public record CompanyResult(
    Guid Id,
    string Name,
    string? TradeName,
    string? CnpjBase,
    string? Cnpj,
    CompanyType Type,
    CompanyStatus Status,
    ClientSegment? Segment,
    CompanySize? SizeCategory,
    string? Email,
    string? Phone,
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
    DateOnly? SimeiSince,
    DateTime CreatedAt,
    DateTime UpdatedAt);
