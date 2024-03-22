using PalpiteFC.Api.Domain.Entities.ApiFootball;
using PalpiteFC.Worker.Integrations.Interfaces;
using System.Text.Json;

namespace PalpiteFC.Worker.Integrations;

public class ApiFootballProvider : IApiFootballProvider
{
    private readonly HttpClient _httpClient;

    public ApiFootballProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Match>> GetMatchesByLeagueId(string leagueId, string fromDate, string toDate)
    {
        var uri = $"/v3/fixtures?league={leagueId}&season=2024&from={fromDate}&to={toDate}";

        var response = await _httpClient.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiFootballResult<Match>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result!.Response!;
    }
    public async Task<Match> GetMatch(int fixtureId)
    {
        var uri = $"/v3/fixtures?id={fixtureId}";

        var response = await _httpClient.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiFootballResult<Match>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result!.Response!.FirstOrDefault(new Match());
    }
}
