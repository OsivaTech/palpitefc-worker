using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using PalpiteFC.Worker.Integrations.Interfaces;
using PalpiteFC.Worker.Integrations.Providers.Responses;
using PalpiteFC.Worker.Integrations.Requests;
using System.Text.Json;

namespace PalpiteFC.Worker.Integrations.Providers;

public class ApiFootballProvider : IApiFootballProvider
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiFootballProvider> _logger;

    public ApiFootballProvider(HttpClient httpClient, ILogger<ApiFootballProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Match>> GetFixtures(FixturesRequest request)
    {
        var queryBuilder = new QueryBuilder();

        if (request.LeagueId > 0) queryBuilder.Add("league", request.LeagueId.ToString());
        if (request.Season > 0) queryBuilder.Add("season", request.Season.ToString());
        if (!string.IsNullOrWhiteSpace(request.Date)) queryBuilder.Add("date", request.Date);
        if (!string.IsNullOrWhiteSpace(request.FromDate)) queryBuilder.Add("from", request.FromDate);
        if (!string.IsNullOrWhiteSpace(request.ToDate)) queryBuilder.Add("to", request.ToDate);
        if (!string.IsNullOrWhiteSpace(request.Timezone)) queryBuilder.Add("timezone", request.Timezone);

        var uri = $"/v3/fixtures{queryBuilder.ToQueryString()}";

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

        if (response.IsSuccessStatusCode is false)
        {
            _logger.LogWarning("ApiFootball retruns is unsuccess: {Content}", content);
            return null!;
        }

        var result = JsonSerializer.Deserialize<ApiFootballResult<Match>>(content, _serializerOptions);

        return result!.Response!.FirstOrDefault(new Match());
    }
}
