using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.AssignUserCompany;

public record AssignUserCompanyCommand(Guid UserId, Guid CompanyId) : IRequest;
