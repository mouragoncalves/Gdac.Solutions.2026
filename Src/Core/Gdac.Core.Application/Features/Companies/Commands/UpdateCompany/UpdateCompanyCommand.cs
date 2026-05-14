using Gdac.Core.Domain.Enums;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand(
    Guid Id, string Name, string? TradeName, string? Cnpj,
    CompanyType Type, string? Email, string? Phone) : IRequest;
