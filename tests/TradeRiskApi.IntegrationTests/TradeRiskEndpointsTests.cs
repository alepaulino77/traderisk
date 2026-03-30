using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TradeRiskApi.IntegrationTests;

public sealed class TradeRiskEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public TradeRiskEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Classify_Should_Return_Expected_Categories()
    {
        var payload = new[]
        {
            new { value = 2_000_000m, clientSector = "Private", clientId = "CLI003" },
            new { value = 400_000m, clientSector = "Public", clientId = "CLI001" },
            new { value = 500_000m, clientSector = "Public", clientId = "CLI001" },
            new { value = 3_000_000m, clientSector = "Public", clientId = "CLI002" }
        };

        var response = await _httpClient.PostAsJsonAsync("/api/trades/risk/classify", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ClassifyResponse>();
        body.Should().NotBeNull();
        body!.Categories.Should().Equal("HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK");
    }

    [Fact]
    public async Task Post_ClassifyWithSummary_Should_Return_ValidationError_When_List_Is_Empty()
    {
        var payload = Array.Empty<object>();

        var response = await _httpClient.PostAsJsonAsync("/api/trades/risk/classify-with-summary", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record ClassifyResponse(IReadOnlyList<string> Categories);
}
