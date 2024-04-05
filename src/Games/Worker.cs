using PalpiteFC.Worker.Integrations.Interfaces;
using PalpiteFC.Worker.Integrations.Requests;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Games;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ILeaguesRepository _leaguesRepository;
    private readonly IFixturesRepository _fixturesRepository;
    private readonly ITeamsRepository _teamsRepository;
    private readonly ITeamsGamesRepository _teamsGamesRepository;
    private readonly IApiFootballProvider _apiFootballProvider;

    public Worker(ILeaguesRepository leaguesRepository,
                  IFixturesRepository fixturesRepository,
                  ITeamsGamesRepository teamsGamesRepository,
                  ITeamsRepository teamsRepository,
                  IApiFootballProvider apiFootballProvider,
                  ILogger<Worker> logger)
    {
        _logger = logger;
        _leaguesRepository = leaguesRepository;
        _fixturesRepository = fixturesRepository;
        _apiFootballProvider = apiFootballProvider;
        _teamsRepository = teamsRepository;
        _teamsGamesRepository = teamsGamesRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow.AddHours(-3);

                var request = new FixturesRequest
                {
                    FromDate = now.ToString("yyyy-MM-dd"),
                    ToDate = now.AddDays(1).ToString("yyyy-MM-dd"),
                    Season = now.Year,
                    Timezone = "America/Sao_Paulo"
                };

                _logger.LogInformation("Retreiving Leagues Ids from database");

                var leagues = _leaguesRepository.Select().Result.Select(x => x.Id);

                if (leagues.Any() is false)
                {
                    _logger.LogWarning("No league id found");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Retreived {count} leagues", leagues.Count());
                _logger.LogInformation("Retreiving fixtures from ApiFootball");

                var tasks = leagues.Select(async champId =>
                {
                    request.LeagueId = champId;
                    return await _apiFootballProvider.GetFixtures(request);
                });

                var matchesArray = await Task.WhenAll(tasks);
                var matchesJoined = matchesArray.Where(x => x is not null).SelectMany(x => x);

                _logger.LogInformation("Retreived {count} fixtures", matchesJoined.Count());
                _logger.LogInformation("Starting data processing");

                var fixtures = new List<Fixtures>();
                var teamsGames = new List<TeamsGame>();
                var teams = new List<Teams>();

                foreach (var item in matchesJoined)
                {
                    var fixtureId = item.Fixture!.Id.GetValueOrDefault();
                    var homeTeamId = item.Teams!.Home!.Id.GetValueOrDefault();
                    var awayTeamId = item.Teams!.Away!.Id.GetValueOrDefault();

                    fixtures.Add(new()
                    {
                        Id = fixtureId,
                        ChampionshipId = item.League!.Id.GetValueOrDefault(),
                        Name = item.League.Name,
                        Start = item.Fixture.Date.GetValueOrDefault().DateTime,
                        Finished = item.Fixture.Status!.Long!.Equals("Match Finished", StringComparison.OrdinalIgnoreCase)
                    });

                    teamsGames.AddRange(new[]
                    {
                    new TeamsGame { GameId = fixtureId, TeamId = homeTeamId, Gol = item.Goals!.Home.GetValueOrDefault() },
                    new TeamsGame { GameId = fixtureId, TeamId = awayTeamId, Gol = item.Goals!.Away.GetValueOrDefault() }
                });

                    teams.AddRange(new[]
                    {
                    new Teams { Id = homeTeamId, Name = item.Teams.Home.Name, Image = item.Teams.Home.Logo },
                    new Teams { Id = awayTeamId, Name = item.Teams.Away.Name, Image = item.Teams.Away.Logo }
                });
                }

                _logger.LogInformation("Saving data on database");

                await Task.WhenAll(
                    _fixturesRepository.InsertOrUpdate(fixtures),
                    _teamsGamesRepository.InsertOrUpdate(teamsGames),
                    _teamsRepository.InsertOrUpdate(teams)
                );

                _logger.LogInformation("Service finished processing");

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

                var timespan = TimeSpan.FromSeconds(30);
                _logger.LogInformation("Restarting service in {time}", timespan);

                await Task.Delay(timespan, stoppingToken);
            }
        }
    }
}
