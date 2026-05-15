using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.RejectRegistration;

public class RejectRegistrationHandler(
    IRegistrationRepository registrationRepo,
    IUnitOfWork uow)
    : IRequestHandler<RejectRegistrationCommand>
{
    public async Task Handle(RejectRegistrationCommand request, CancellationToken ct)
    {
        var registration = await registrationRepo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Cadastro", request.Id);

        registration.Reject(request.ReviewedBy, request.Notes);
        registrationRepo.Update(registration);
        await uow.CommitAsync(ct);
    }
}
