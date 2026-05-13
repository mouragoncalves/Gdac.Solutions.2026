using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.UpdateCompany;

public record UpdateCompanyCommand(Guid CompanyId, string Name, bool IsActive) : IRequest;
