using Microsoft.Extensions.Options;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Games.Interfaces;
using PalpiteFC.Worker.Games.Settings;
using PalpiteFC.Worker.Integrations.Interfaces;
using PalpiteFC.Worker.Integrations.Requests;

namespace PalpiteFC.Worker.Games.Services;

public class FixturesService : IFixturesService
{
    private readonly ILeaguesRepository _leaguesRepository;
    private readonly IFixturesRepository _fixturesRepository;
    private readonly ITeamsRepository _teamsRepository;
    private readonly IMatchesRepository _matchesRepository;
    private readonly IApiFootballProvider _apiFootballProvider;
    private readonly IOptions<WorkerSettings> _options;
    private readonly ILogger<FixturesService> _logger;

    public FixturesService(ILeaguesRepository leaguesRepository,
                           IFixturesRepository fixturesRepository,
                           ITeamsRepository teamsRepository,
                           IMatchesRepository matchesRepository,
                           IApiFootballProvider apiFootballProvider,
                           IOptions<WorkerSettings> options,
                           ILogger<FixturesService> logger)
    {
        _leaguesRepository = leaguesRepository;
        _fixturesRepository = fixturesRepository;
        _teamsRepository = teamsRepository;
        _matchesRepository = matchesRepository;
        _apiFootballProvider = apiFootballProvider;
        _options = options;
        _logger = logger;
    }

    public async Task<bool> TryProcessAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting loading fixtures");

        var daysToSearch = _options.Value.DaysToSearch;

        var dates = Enumerable.Range(0, daysToSearch)
                              .Select(i => DateTime.Now.AddDays(i))
                              .ToList();

        _logger.LogInformation("Retreiving Leagues Ids from database");

        var leagues = _leaguesRepository.SelectEnabled().Result.Select(x => x.ExternalId);

        if (leagues.Any() is false)
        {
            _logger.LogWarning("No league id found");
            return false;
        }

        _logger.LogInformation("Retreived {LeagueCount} leagues", leagues.Count());
        _logger.LogInformation("Retreiving fixtures from ApiFootball");

        var tasks = dates.Select(async date =>
        {
            var request = new FixturesRequest
            {
                Date = date.ToString("yyyy-MM-dd"),
                Timezone = "America/Sao_Paulo"
            };

            var response = await _apiFootballProvider.GetFixtures(request);

            return response.Where(x => leagues.Contains(x.League?.Id ?? 0));
        });

        var matchesArray = await Task.WhenAll(tasks);
        var matchesJoined = matchesArray.Where(x => x is not null).SelectMany(x => x);

        _logger.LogInformation("Retreived {FixtureCount} fixtures", matchesJoined.Count());
        _logger.LogInformation("Starting data processing");

        var fixtures = new List<Fixture>();
        var matches = new List<Match>();
        var teams = new List<Team>();

        foreach (var item in matchesJoined)
        {
            var fixtureId = item.Fixture?.Id.GetValueOrDefault() ?? 0;
            var homeId = item.Teams?.Home?.Id.GetValueOrDefault() ?? 0;
            var awayId = item.Teams?.Away?.Id.GetValueOrDefault() ?? 0;

            fixtures.Add(new()
            {
                ExternalId = fixtureId,
                DataSourceId = 1,
                LeagueId = item.League?.Id.GetValueOrDefault() ?? 0,
                Name = item.League?.Name,
                Start = item.Fixture?.Date.GetValueOrDefault().UtcDateTime ?? DateTime.MinValue,
                Finished = item.Fixture?.Status?.Long?.Equals("Match Finished", StringComparison.OrdinalIgnoreCase) ?? false
            });

            matches.Add(new()
            {
                FixtureId = fixtureId,
                DataSourceId = 1,
                HomeId = homeId,
                AwayId = awayId,
                HomeGoals = item.Goals?.Home.GetValueOrDefault() ?? 0,
                AwayGoals = item.Goals?.Away.GetValueOrDefault() ?? 0
            });

            teams.AddRange(new[]
            {
                    new Team { ExternalId = homeId, DataSourceId = 1, Name = item.Teams?.Home?.Name, Image = item.Teams?.Home?.Logo },
                    new Team { ExternalId = awayId, DataSourceId = 1, Name = item.Teams?.Away?.Name, Image = item.Teams?.Away?.Logo }
                });
        }

        _logger.LogInformation("Saving data on database");

        await _teamsRepository.InsertOrUpdate(teams);
        await _fixturesRepository.InsertOrUpdate(fixtures);
        await _matchesRepository.InsertOrUpdate(matches);

        _logger.LogInformation("Done!");

        return true;
    }
}
