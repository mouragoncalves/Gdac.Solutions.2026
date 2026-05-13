using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateCompany;

public record CreateCompanyCommand(string ExternalId, string Name) : IRequest<CreateCompanyResult>;
public record CreateCompanyResult(Guid Id, string ExternalId, string Name);
