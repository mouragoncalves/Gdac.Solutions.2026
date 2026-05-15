namespace Gdac.Onboarding.Domain.Interfaces.Services;

public interface ICoreApiClient
{
    Task<bool> IsCnpjBlockedAsync(string cnpjBase, CancellationToken ct = default);
    Task<Guid> CreateCompanyAsync(CreateCoreCompanyRequest request, CancellationToken ct = default);
    Task CreateUserProfileAsync(Guid userId, string fullName, string email, CancellationToken ct = default);
    Task LinkUserToCompanyAsync(Guid companyId, Guid userId, string role, CancellationToken ct = default);
}

public record CreateCoreCompanyRequest(
    string Name, int Type, string? TradeName, string? Cnpj,
    string? Email, string? Phone, int? Segment, int? SizeCategory);
