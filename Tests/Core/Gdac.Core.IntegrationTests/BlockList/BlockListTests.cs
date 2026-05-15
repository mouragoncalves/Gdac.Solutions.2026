using FluentAssertions;
using Gdac.Core.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Gdac.Core.IntegrationTests.BlockList;

[Collection("Core Integration")]
public class BlockListTests(CoreWebAppFactory factory)
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
    public async Task BlockCnpj_Unauthenticated_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/block-list", new { cnpjBase = "12345678", reason = (string?)null });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BlockCnpj_ValidRequest_Returns204()
    {
        var client = AuthenticatedClient();
        var cnpjBase = GenerateUniqueCnpjBase();
        var payload = new { cnpjBase, reason = "fraude" };

        var response = await client.PostAsJsonAsync("/block-list", payload);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task BlockCnpj_DuplicateBase_Returns400()
    {
        var client = AuthenticatedClient();
        var cnpjBase = GenerateUniqueCnpjBase();
        var payload = new { cnpjBase, reason = (string?)null };

        await client.PostAsJsonAsync("/block-list", payload);
        var response = await client.PostAsJsonAsync("/block-list", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBlockList_Authenticated_Returns200()
    {
        var client = AuthenticatedClient();
        var response = await client.GetAsync("/block-list");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckCnpj_BlockedBase_ReturnsTrue()
    {
        var client = AuthenticatedClient();
        var cnpjBase = GenerateUniqueCnpjBase();
        await client.PostAsJsonAsync("/block-list", new { cnpjBase, reason = (string?)null });

        var response = await client.GetAsync($"/block-list/{cnpjBase}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var isBlocked = await response.Content.ReadFromJsonAsync<bool>();
        isBlocked.Should().BeTrue();
    }

    [Fact]
    public async Task UnblockCnpj_ExistingBase_Returns204()
    {
        var client = AuthenticatedClient();
        var cnpjBase = GenerateUniqueCnpjBase();
        await client.PostAsJsonAsync("/block-list", new { cnpjBase, reason = (string?)null });

        var response = await client.DeleteAsync($"/block-list/{cnpjBase}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UnblockCnpj_NonExistentBase_Returns404()
    {
        var client = AuthenticatedClient();
        var response = await client.DeleteAsync("/block-list/00000000");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static string GenerateUniqueCnpjBase()
    {
        var random = new Random();
        return string.Concat(Enumerable.Range(0, 8).Select(_ => random.Next(0, 10).ToString()));
    }
}
