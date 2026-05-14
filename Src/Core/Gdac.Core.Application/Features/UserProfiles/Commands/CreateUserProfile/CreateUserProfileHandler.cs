using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Commands.CreateUserProfile;

public class CreateUserProfileHandler(IUserProfileRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateUserProfileCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserProfileCommand request, CancellationToken ct)
    {
        if (await repo.ExistsAsync(request.UserId, ct))
            throw new DomainException("Perfil já existe para este usuário.");

        var profile = UserProfile.Create(request.UserId, request.FullName, request.Email);
        await repo.AddAsync(profile, ct);
        await uow.CommitAsync(ct);
        return profile.Id;
    }
}
