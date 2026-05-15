using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.ApproveRegistration;

public record ApproveRegistrationCommand(Guid Id, Guid ReviewedBy, string? Notes) : IRequest<ApproveRegistrationResult>;

public record ApproveRegistrationResult(Guid CompanyId, Guid UserId, string TemporaryPassword);
