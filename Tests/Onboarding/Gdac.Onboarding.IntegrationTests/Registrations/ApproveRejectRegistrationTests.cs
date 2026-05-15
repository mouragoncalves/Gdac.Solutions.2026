using FluentAssertions;
using Gdac.Onboarding.Domain.Interfaces.Services;
using Gdac.Onboarding.IntegrationTests.Infrastructure;
using NSubstitute;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Gdac.Onboarding.IntegrationTests.Registrations;

public class ApproveRejectRegistrationTests(OnboardingWebAppFactory factory)
    : IClassFixture<OnboardingWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly Guid _adminId = Guid.NewGuid();

    private HttpClient AuthenticatedClient()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwtFactory.ForUser(_adminId));
        return client;
    }

    private async Task<Guid> SubmitClientAsync()
    {
        factory.CoreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var cnpj = GenerateUniqueCnpj();
        var payload = new
        {
            contactName = "Test User",
            contactEmail = $"test.{Guid.NewGuid():N}@empresa.com",
            companyName = "Empresa Test",
            cnpj
        };

        var response = await _client.PostAsJsonAsync("/registrations/client", payload);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task ApproveRegistration_ValidId_Returns200WithResult()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        factory.CoreApi.CreateCompanyAsync(Arg.Any<CreateCoreCompanyRequest>(), Arg.Any<CancellationToken>())
            .Returns(companyId);
        factory.AuthApi.CreateUserAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new CreateAuthUserResult(userId, "Temp@1234"));
        factory.CoreApi.CreateUserProfileAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        factory.CoreApi.LinkUserToCompanyAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var id = await SubmitClientAsync();
        var client = AuthenticatedClient();

        var response = await client.PostAsJsonAsync($"/registrations/{id}/approve", new { notes = "OK" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApproveResult>();
        result!.CompanyId.Should().Be(companyId);
        result.UserId.Should().Be(userId);
        result.TemporaryPassword.Should().Be("Temp@1234");
    }

    [Fact]
    public async Task RejectRegistration_ValidId_Returns204()
    {
        var id = await SubmitClientAsync();
        var client = AuthenticatedClient();

        var response = await client.PostAsJsonAsync($"/registrations/{id}/reject",
            new { notes = "Documentação incompleta" });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ApproveRegistration_NonExistentId_Returns404()
    {
        var client = AuthenticatedClient();
        var response = await client.PostAsJsonAsync($"/registrations/{Guid.NewGuid()}/approve",
            new { });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApproveRegistration_Unauthenticated_Returns401()
    {
        var response = await _client.PostAsJsonAsync($"/registrations/{Guid.NewGuid()}/approve",
            new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static string GenerateUniqueCnpj()
    {
        var random = new Random();
        return string.Concat(Enumerable.Range(0, 14).Select(_ => random.Next(0, 10).ToString()));
    }

    private record IdResponse(Guid Id);
    private record ApproveResult(Guid CompanyId, Guid UserId, string TemporaryPassword);
}
