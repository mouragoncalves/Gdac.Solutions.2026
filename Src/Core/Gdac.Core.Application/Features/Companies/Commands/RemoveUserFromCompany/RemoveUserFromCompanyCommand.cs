using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.RemoveUserFromCompany;

public record RemoveUserFromCompanyCommand(Guid CompanyId, Guid UserId) : IRequest;
