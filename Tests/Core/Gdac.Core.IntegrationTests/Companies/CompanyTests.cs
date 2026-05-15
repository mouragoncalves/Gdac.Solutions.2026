using FluentAssertions;
using Gdac.Core.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Gdac.Core.IntegrationTests.Companies;

[Collection("Core Integration")]
public class CompanyTests(CoreWebAppFactory factory)
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
    public async Task GetCompanies_Unauthenticated_Returns401()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/companies");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCompanies_Authenticated_Returns200()
    {
        var client = AuthenticatedClient();
        var response = await client.GetAsync("/companies");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCompany_ValidRequest_Returns201()
    {
        var client = AuthenticatedClient();
        var payload = new
        {
            name = "Empresa Integração LTDA",
            type = 0,
            tradeName = (string?)null,
            cnpj = (string?)null,
            email = (string?)null,
            phone = (string?)null,
            segment = (int?)null,
            sizeCategory = (int?)null
        };

        var response = await client.PostAsJsonAsync("/companies", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateCompany_EmptyName_Returns400()
    {
        var client = AuthenticatedClient();
        var payload = new { name = "", type = 0 };

        var response = await client.PostAsJsonAsync("/companies", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCompany_ValidRequest_Returns204()
    {
        var client = AuthenticatedClient();
        var createPayload = new { name = "Empresa Update", type = 0 };
        var createResponse = await client.PostAsJsonAsync("/companies", createPayload);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var updatePayload = new
        {
            name = "Empresa Atualizada",
            tradeName = (string?)null,
            cnpj = (string?)null,
            type = 1,
            email = (string?)null,
            phone = (string?)null,
            segment = (int?)null,
            sizeCategory = (int?)null
        };

        var response = await client.PutAsJsonAsync($"/companies/{created!.Id}", updatePayload);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeactivateCompany_ValidId_Returns204()
    {
        var client = AuthenticatedClient();
        var createPayload = new { name = "Empresa Desativar", type = 0 };
        var createResponse = await client.PostAsJsonAsync("/companies", createPayload);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await client.DeleteAsync($"/companies/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateCompany_NonExistentId_Returns404()
    {
        var client = AuthenticatedClient();
        var payload = new
        {
            name = "Empresa",
            tradeName = (string?)null,
            cnpj = (string?)null,
            type = 0,
            email = (string?)null,
            phone = (string?)null,
            segment = (int?)null,
            sizeCategory = (int?)null
        };

        var response = await client.PutAsJsonAsync($"/companies/{Guid.NewGuid()}", payload);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private record IdResponse(Guid Id);
}
