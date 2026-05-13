using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.UpdateApplication;

public class UpdateApplicationHandler(
    IApplicationRepository applications,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateApplicationCommand>
{
    public async Task Handle(UpdateApplicationCommand cmd, CancellationToken ct)
    {
        var app = await applications.FindByIdAsync(cmd.ApplicationId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Application), cmd.ApplicationId);

        app.Update(cmd.Name);
        if (cmd.IsActive) app.Activate(); else app.Deactivate();

        applications.Update(app);
        await unitOfWork.CommitAsync(ct);
    }
}
