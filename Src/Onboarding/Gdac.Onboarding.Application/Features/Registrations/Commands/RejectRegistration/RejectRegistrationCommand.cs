using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.RejectRegistration;

public record RejectRegistrationCommand(Guid Id, Guid ReviewedBy, string? Notes) : IRequest;
