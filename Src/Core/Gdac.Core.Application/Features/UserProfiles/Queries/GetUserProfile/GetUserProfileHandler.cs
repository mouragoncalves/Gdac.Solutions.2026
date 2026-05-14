using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfile;

public class GetUserProfileHandler(IUserProfileRepository repo)
    : IRequestHandler<GetUserProfileQuery, UserProfileResult>
{
    public async Task<UserProfileResult> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var profile = await repo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("Perfil", request.UserId);

        return new UserProfileResult(profile.Id, profile.FullName, profile.Email,
            profile.Phone, profile.AvatarUrl, profile.Cpf,
            profile.BirthDate, profile.IsActive,
            profile.CreatedAt, profile.UpdatedAt);
    }
}
