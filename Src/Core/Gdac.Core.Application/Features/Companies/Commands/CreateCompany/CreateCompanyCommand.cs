using Gdac.Core.Domain.Enums;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(
    string Name, CompanyType Type,
    string? TradeName, string? Cnpj,
    string? Email, string? Phone,
    ClientSegment? Segment, CompanySize? SizeCategory) : IRequest<Guid>;
