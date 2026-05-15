using FluentAssertions;
using Gdac.Core.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Gdac.Core.IntegrationTests.UserProfiles;

[Collection("Core Integration")]
public class UserProfileTests(CoreWebAppFactory factory)
{
    private readonly Guid _userId = Guid.NewGuid();

    private HttpClient AuthenticatedClient()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwtFactory.ForUser(_userId));
        return client;
    }

    [Fact]
    public async Task GetProfiles_Unauthenticated_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/users");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfiles_Authenticated_Returns200()
    {
        var client = AuthenticatedClient();
        var response = await client.GetAsync("/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProfile_ValidRequest_Returns201()
    {
        var client = AuthenticatedClient();
        var userId = Guid.NewGuid();
        var payload = new
        {
            userId,
            fullName = "Usuário Teste",
            email = $"usuario.{Guid.NewGuid():N}@gdac.com"
        };

        var response = await client.PostAsJsonAsync("/users", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task CreateProfile_DuplicateUserId_Returns400()
    {
        var client = AuthenticatedClient();
        var userId = Guid.NewGuid();
        var payload = new
        {
            userId,
            fullName = "Usuário Duplicado",
            email = $"dup.{Guid.NewGuid():N}@gdac.com"
        };

        await client.PostAsJsonAsync("/users", payload);
        var response = await client.PostAsJsonAsync("/users", payload with { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProfileById_ExistingId_Returns200()
    {
        var client = AuthenticatedClient();
        var userId = Guid.NewGuid();
        var payload = new
        {
            userId,
            fullName = "Buscar Por Id",
            email = $"buscar.{Guid.NewGuid():N}@gdac.com"
        };

        await client.PostAsJsonAsync("/users", payload);

        var response = await client.GetAsync($"/users/{userId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private record IdResponse(Guid Id);
}
