using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Commands.UpdateUserProfile;

public class UpdateUserProfileHandler(IUserProfileRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateUserProfileCommand>
{
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken ct)
    {
        var profile = await repo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("Perfil", request.UserId);

        profile.Update(request.FullName, request.Phone, request.AvatarUrl, request.Cpf, request.BirthDate);
        repo.Update(profile);
        await uow.CommitAsync(ct);
    }
}
