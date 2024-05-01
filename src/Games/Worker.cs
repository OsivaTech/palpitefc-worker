using Microsoft.Extensions.Options;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Games.Settings;
using PalpiteFC.Worker.Integrations.Interfaces;
using PalpiteFC.Worker.Integrations.Requests;

namespace PalpiteFC.Worker.Games;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ILeaguesRepository _leaguesRepository;
    private readonly IFixturesRepository _fixturesRepository;
    private readonly ITeamsRepository _teamsRepository;
    private readonly IMatchesRepository _matchesRepository;
    private readonly IApiFootballProvider _apiFootballProvider;
    private readonly IOptions<WorkerSettings> _options;

    public Worker(ILeaguesRepository leaguesRepository,
                  IFixturesRepository fixturesRepository,
                  IMatchesRepository matchesRepository,
                  ITeamsRepository teamsRepository,
                  IApiFootballProvider apiFootballProvider,
                  ILogger<Worker> logger,
                  IOptions<WorkerSettings> options)
    {
        _logger = logger;
        _leaguesRepository = leaguesRepository;
        _fixturesRepository = fixturesRepository;
        _apiFootballProvider = apiFootballProvider;
        _teamsRepository = teamsRepository;
        _matchesRepository = matchesRepository;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var daysToSearch = _options.Value.DaysToSearch;

                var dates = Enumerable.Range(0, daysToSearch)
                                      .Select(i => DateTime.Now.AddDays(i))
                                      .ToList();

                _logger.LogInformation("Retreiving Leagues Ids from database");

                var leagues = _leaguesRepository.Select().Result.Select(x => x.Id);

                if (leagues.Any() is false)
                {
                    _logger.LogWarning("No league id found");
                    await Task.Delay(_options.Value.RestartDelay, stoppingToken);
                    continue;
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
                    var homeTeamId = item.Teams?.Home?.Id.GetValueOrDefault() ?? 0;
                    var awayTeamId = item.Teams?.Away?.Id.GetValueOrDefault() ?? 0;

                    fixtures.Add(new()
                    {
                        Id = fixtureId,
                        LeagueId = item.League?.Id.GetValueOrDefault() ?? 0,
                        Name = item.League?.Name,
                        Start = item.Fixture?.Date.GetValueOrDefault().DateTime ?? DateTime.MinValue,
                        Finished = item.Fixture?.Status?.Long?.Equals("Match Finished", StringComparison.OrdinalIgnoreCase) ?? false
                    });

                    matches.Add(new()
                    {
                        FixtureId = fixtureId,
                        HomeId = homeTeamId,
                        AwayId = awayTeamId,
                        HomeGoals = item.Goals?.Home.GetValueOrDefault() ?? 0,
                        AwayGoals = item.Goals?.Away.GetValueOrDefault() ?? 0
                    });

                    teams.AddRange(new[]
                    {
                    new Team { Id = homeTeamId, Name = item.Teams?.Home?.Name, Image = item.Teams?.Home?.Logo },
                    new Team { Id = awayTeamId, Name = item.Teams?.Away?.Name, Image = item.Teams?.Away?.Logo }
                });
                }

                _logger.LogInformation("Saving data on database");

                await Task.WhenAll(
                    _fixturesRepository.InsertOrUpdate(fixtures),
                    _matchesRepository.InsertOrUpdate(matches),
                    _teamsRepository.InsertOrUpdate(teams)
                );

                _logger.LogInformation("Service finished processing! Starting again in {LoopDelay}.", _options.Value.LoopDelay);
                await Task.Delay(_options.Value.LoopDelay, stoppingToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                _logger.LogInformation("Restarting service in {Time}", _options.Value.RestartDelay);

                await Task.Delay(_options.Value.RestartDelay, stoppingToken);
            }
        }
    }
}
