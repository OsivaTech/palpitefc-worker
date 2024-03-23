using PalpiteFC.Worker.Integrations.Entities;
using PalpiteFC.Worker.Integrations.Interfaces;
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

    public async Task<IEnumerable<Match>> GetMatchesByLeagueId(int leagueId, int season, string fromDate, string toDate)
    {
        var uri = $"/v3/fixtures?league={leagueId}&season={season}&from={fromDate}&to={toDate}&timezone=America/Sao_Paulo";

        var response = await _httpClient.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiFootballResult<Match>>(content, _serializerOptions);

        return result!.Response!;
    }
    public async Task<Match> GetMatch(int fixtureId)
    {
        var uri = $"/v3/fixtures?id={fixtureId}";

        var response = await _httpClient.GetAsync(uri);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ApiFootballResult<Match>>(content, _serializerOptions);

        return result!.Response!.FirstOrDefault(new Match());
    }
}
