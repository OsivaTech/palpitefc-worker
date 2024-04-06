using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Integrations.Interfaces;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Guesses;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Queue<Fixtures> _queue = new();
    private readonly IFixturesRepository _fixturesRepository;
    private readonly IApiFootballProvider _apiFootballProvider;
    private readonly IGuessesRepository _guessesRepository;
    private readonly IUserPointsRepository _userPointsRepository;
    private readonly IPointsService _pointsService;

    private static readonly string[] finishedStatus = ["PEN", "AET", "FT"];

    public Worker(IApiFootballProvider apiFootballProvider,
                  IGuessesRepository guessesRepository,
                  IUserPointsRepository userPointsRepository,
                  IFixturesRepository fixturesRepository,
                  IPointsService pointsService,
                  ILogger<Worker> logger)
    {
        _apiFootballProvider = apiFootballProvider;
        _guessesRepository = guessesRepository;
        _userPointsRepository = userPointsRepository;
        _fixturesRepository = fixturesRepository;
        _pointsService = pointsService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Finding for fixtures...");

                var date = DateTime.Now.Date;
                var fixtures = await _fixturesRepository.Select(date, date.AddDays(1).AddTicks(-1));

                _logger.LogInformation("Found {FixtureCount} fixtures", fixtures.Count());

                fixtures.ToList().ForEach(_queue.Enqueue);

                while (_queue.TryDequeue(out var fixture))
                {
                    if (fixture.Start >= DateTime.Now.AddMinutes(-90))
                    {
                        _queue.Enqueue(fixture);
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                        continue;
                    }

                    _ = Task.Run(async () => await ProcessGuesses(fixture.Id, stoppingToken), stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

                var timespan = TimeSpan.FromSeconds(30);
                _logger.LogInformation("Restarting service in {Time}", timespan);

                await Task.Delay(timespan, stoppingToken);
            }
        }
    }

    private async Task ProcessGuesses(int id, CancellationToken stoppingToken)
    {
        while (true)
        {
            _logger.LogInformation("Retreiving fixture {FixtureId} informations.", id);
            var fixture = await _apiFootballProvider.GetFixture(id);

            if (fixture is null)
            {
                _logger.LogInformation("Fixture {FixtureId} was not found. Breaking operation.", id);
                break;
            }

            _logger.LogInformation("Processing fixture {FixtureId} - {League} - {HomeTeam} vs {AwayTeam}",
                    id, fixture.League?.Name, fixture.Teams?.Home?.Name, fixture.Teams?.Away?.Name);

            if (fixture.Fixture?.Status?.Short?.Equals("NS", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                _logger.LogInformation("Fixture {FixtureId} not started. Breaking operation.", id);
                break;
            }

            if (IsFinished(fixture.Fixture?.Status?.Short ?? string.Empty))
            {
                var guesses = await _guessesRepository.SelectByFixtureId(id);

                if (guesses.Any() is false)
                {
                    _logger.LogInformation("No guesses found for fixture {FixtureId}. Breaking operation.", id);
                    break;
                }

                guesses.ToList().ForEach(async guess =>
                {
                    var earnedPoints = await _pointsService.CalculatePoints(guess, fixture);

                    if (earnedPoints > 0)
                    {
                        await _userPointsRepository.Insert(new()
                        {
                            UserId = guess.UserId,
                            GameId = guess.GameId,
                            Points = earnedPoints
                        });
                    }
                });

                break;
            }
            _logger.LogInformation("Fixture {FixtureId} not finished yet, trying again soon", id);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            static bool IsFinished(string status) => finishedStatus.Contains(status);
        }
    }
}
