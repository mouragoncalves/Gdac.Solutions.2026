using Gdac.Onboarding.Domain.Enums;
using Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistration;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistrations;

public record GetRegistrationsQuery(RegistrationStatus? Status = null) : IRequest<IReadOnlyList<RegistrationResult>>;
