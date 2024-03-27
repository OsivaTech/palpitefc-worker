using PalpiteFC.Worker.Integrations.Entities;
using PalpiteFC.Worker.Integrations.Interfaces;
using PalpiteFC.Worker.Integrations.Requests;
using System.Text.Json;

namespace PalpiteFC.Worker.Integrations.Providers;

public class ApiFootballProvider : IApiFootballProvider
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _httpClient;

    public ApiFootballProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Match>> GetFixtures(FixturesRequest request)
    {
        var uri = $"/v3/fixtures?league={request.LeagueId}&season={request.Season}&from={request.FromDate}&to={request.ToDate}&timezone={request.Timezone}";

        var response = await _httpClient.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiFootballResult<Match>>(content, _serializerOptions);

        return result!.Response!;
    }

    public async Task<Match> GetFixture(int fixtureId)
    {
        var uri = $"/v3/fixtures?id={fixtureId}";

        var response = await _httpClient.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiFootballResult<Match>>(content, _serializerOptions);

        return result!.Response!.FirstOrDefault(new Match());
    }
}
