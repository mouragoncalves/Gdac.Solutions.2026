using FluentAssertions;
using Gdac.Onboarding.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Gdac.Onboarding.IntegrationTests.LeadDistribution;

[Collection("Onboarding Integration")]
public class LeadDistributionConfigTests(OnboardingWebAppFactory factory)
{
    private HttpClient AuthenticatedClient()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwtFactory.ForUser(Guid.NewGuid()));
        return client;
    }

    [Fact]
    public async Task GetConfig_Unauthenticated_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/lead-distribution-config");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConfig_NoConfigYet_Returns200WithNull()
    {
        var client = AuthenticatedClient();
        var response = await client.GetAsync("/lead-distribution-config");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateConfig_ValidPayload_Returns204()
    {
        var client = AuthenticatedClient();
        var payload = new { mode = 1 }; // Manual

        var response = await client.PutAsJsonAsync("/lead-distribution-config", payload);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateThenGet_ReturnsUpdatedConfig()
    {
        var client = AuthenticatedClient();
        var partnerId = Guid.NewGuid();

        await client.PutAsJsonAsync("/lead-distribution-config",
            new { mode = 2, defaultPartnerId = partnerId }); // RevendaPadrao

        var getResponse = await client.GetAsync("/lead-distribution-config");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var config = await getResponse.Content.ReadFromJsonAsync<ConfigResult>();
        config.Should().NotBeNull();
        config!.Mode.Should().Be(2);
        config.DefaultPartnerId.Should().Be(partnerId);
    }

    private record ConfigResult(Guid Id, int Mode, Guid? DefaultPartnerId);
}
