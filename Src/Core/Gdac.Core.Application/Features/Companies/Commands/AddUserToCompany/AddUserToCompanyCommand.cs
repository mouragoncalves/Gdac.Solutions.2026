using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.AddUserToCompany;

public record AddUserToCompanyCommand(Guid CompanyId, Guid UserId, string Role) : IRequest;
