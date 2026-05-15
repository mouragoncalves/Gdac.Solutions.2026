using FluentAssertions;
using Gdac.Onboarding.Domain.Interfaces.Services;
using Gdac.Onboarding.IntegrationTests.Infrastructure;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace Gdac.Onboarding.IntegrationTests.Registrations;

public class SubmitRegistrationTests(OnboardingWebAppFactory factory)
    : IClassFixture<OnboardingWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task SubmitClient_ValidRequest_Returns201()
    {
        factory.CoreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var payload = new
        {
            contactName = "João Silva",
            contactEmail = $"joao.{Guid.NewGuid():N}@empresa.com",
            companyName = "Empresa Teste LTDA",
            cnpj = "12345678000195"
        };

        var response = await _client.PostAsJsonAsync("/registrations/client", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SubmitClient_BlockedCnpj_Returns400()
    {
        factory.CoreApi.IsCnpjBlockedAsync("12345678", Arg.Any<CancellationToken>())
            .Returns(true);

        var payload = new
        {
            contactName = "João",
            contactEmail = "joao@empresa.com",
            companyName = "Empresa Bloqueada",
            cnpj = "12345678000195"
        };

        var response = await _client.PostAsJsonAsync("/registrations/client", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmitClient_InvalidCnpj_Returns400WithValidationErrors()
    {
        var payload = new
        {
            contactName = "João",
            contactEmail = "joao@empresa.com",
            companyName = "Empresa",
            cnpj = "123"
        };

        var response = await _client.PostAsJsonAsync("/registrations/client", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmitPartner_ValidRequest_Returns201()
    {
        factory.CoreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var payload = new
        {
            contactName = "Carlos Parceiro",
            contactEmail = $"carlos.{Guid.NewGuid():N}@parceiro.com",
            companyName = "Parceiro LTDA",
            cnpj = "98765432000110"
        };

        var response = await _client.PostAsJsonAsync("/registrations/partner", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task SubmitClient_DuplicateCnpj_Returns400()
    {
        factory.CoreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var email = $"dup.{Guid.NewGuid():N}@empresa.com";
        var payload = new
        {
            contactName = "Duplicado",
            contactEmail = email,
            companyName = "Empresa Dup",
            cnpj = "11222333000181"
        };

        // primeira submissão
        await _client.PostAsJsonAsync("/registrations/client", payload);

        // segunda com mesmo CNPJ
        var response = await _client.PostAsJsonAsync("/registrations/client",
            payload with { contactEmail = $"outro.{Guid.NewGuid():N}@empresa.com" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRegistrations_Unauthenticated_Returns401()
    {
        var response = await _client.GetAsync("/registrations");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRegistrations_Authenticated_Returns200()
    {
        var token = TestJwtFactory.ForUser(Guid.NewGuid());
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/registrations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    private record IdResponse(Guid Id);
}
