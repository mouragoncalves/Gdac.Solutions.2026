using Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfile;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfiles;

public class GetUserProfilesHandler(IUserProfileRepository repo)
    : IRequestHandler<GetUserProfilesQuery, IReadOnlyList<UserProfileResult>>
{
    public async Task<IReadOnlyList<UserProfileResult>> Handle(GetUserProfilesQuery request, CancellationToken ct)
    {
        var profiles = await repo.GetAllAsync(ct);
        return profiles.Select(p => new UserProfileResult(p.Id, p.FullName, p.Email,
            p.Phone, p.AvatarUrl, p.Cpf, p.BirthDate, p.IsActive,
            p.CreatedAt, p.UpdatedAt)).ToList();
    }
}
