using Gdac.Core.Application.Features.Companies.Queries.GetCompany;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanies;

public record GetCompaniesQuery : IRequest<IReadOnlyList<CompanyResult>>;
