namespace Gdac.Onboarding.Domain.Interfaces.Services;

public interface IAuthApiClient
{
    Task<CreateAuthUserResult> CreateUserAsync(string email, CancellationToken ct = default);
    Task<Guid> CreateCompanyAsync(string externalId, string name, CancellationToken ct = default);
    Task AssignUserToCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default);
}

public record CreateAuthUserResult(Guid UserId, string TemporaryPassword);
