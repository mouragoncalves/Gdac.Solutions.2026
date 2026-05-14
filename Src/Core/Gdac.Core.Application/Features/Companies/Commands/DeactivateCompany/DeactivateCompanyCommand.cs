using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.DeactivateCompany;

public record DeactivateCompanyCommand(Guid Id) : IRequest;
