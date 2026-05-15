using Gdac.Onboarding.Domain.Interfaces.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gdac.Onboarding.Infrastructure.Services;

public class CoreApiClient(IHttpClientFactory httpClientFactory) : ICoreApiClient
{
    private static readonly JsonSerializerOptions JsonOpts =
        new(JsonSerializerDefaults.Web);

    private HttpClient Client => httpClientFactory.CreateClient("core-api");

    public async Task<bool> IsCnpjBlockedAsync(string cnpjBase, CancellationToken ct = default)
    {
        var response = await Client.GetAsync($"block-list/{cnpjBase}", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>(JsonOpts, ct);
    }

    public async Task<Guid> CreateCompanyAsync(CreateCoreCompanyRequest request, CancellationToken ct = default)
    {
        var response = await Client.PostAsJsonAsync("companies", request, JsonOpts, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IdResult>(JsonOpts, ct);
        return result!.Id;
    }

    public async Task CreateUserProfileAsync(Guid userId, string fullName, string email, CancellationToken ct = default)
    {
        var payload = new { userId, fullName, email };
        var response = await Client.PostAsJsonAsync("user-profiles", payload, JsonOpts, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task LinkUserToCompanyAsync(Guid companyId, Guid userId, string role, CancellationToken ct = default)
    {
        var payload = new { userId, role };
        var response = await Client.PostAsJsonAsync($"companies/{companyId}/users", payload, JsonOpts, ct);
        response.EnsureSuccessStatusCode();
    }

    private record IdResult(Guid Id);
}
