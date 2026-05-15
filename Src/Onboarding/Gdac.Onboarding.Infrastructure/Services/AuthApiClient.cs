using Gdac.Onboarding.Domain.Interfaces.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gdac.Onboarding.Infrastructure.Services;

public class AuthApiClient(IHttpClientFactory httpClientFactory) : IAuthApiClient
{
    private static readonly JsonSerializerOptions JsonOpts =
        new(JsonSerializerDefaults.Web);

    private HttpClient Client => httpClientFactory.CreateClient("auth-api");

    public async Task<CreateAuthUserResult> CreateUserAsync(string email, CancellationToken ct = default)
    {
        var payload = new { email };
        var response = await Client.PostAsJsonAsync("users", payload, JsonOpts, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>(JsonOpts, ct);
        return new CreateAuthUserResult(result!.UserId, result.TemporaryPassword);
    }

    public async Task<Guid> CreateCompanyAsync(string externalId, string name, CancellationToken ct = default)
    {
        var payload = new { externalId, name };
        var response = await Client.PostAsJsonAsync("companies", payload, JsonOpts, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IdResult>(JsonOpts, ct);
        return result!.Id;
    }

    public async Task AssignUserToCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        var payload = new { userId };
        var response = await Client.PostAsJsonAsync($"companies/{companyId}/users", payload, JsonOpts, ct);
        response.EnsureSuccessStatusCode();
    }

    private record CreateUserResponse(Guid UserId, string TemporaryPassword);
    private record IdResult(Guid Id);
}
