using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.RemoveUserCompany;

public record RemoveUserCompanyCommand(Guid UserId, Guid CompanyId) : IRequest;
