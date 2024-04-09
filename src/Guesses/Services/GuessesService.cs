using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Integrations.Interfaces;

namespace PalpiteFC.Worker.Guesses.Services;

public class GuessesService : IGuessesService
{
    #region Fields

    private readonly ILogger<GuessesService> _logger;
    private readonly IApiFootballProvider _apiFootballProvider;
    private readonly IGuessesRepository _guessesRepository;
    private readonly IUserPointsRepository _userPointsRepository;
    private readonly IPointsService _pointsService;

    private static readonly string[] finishedStatus = ["PEN", "AET", "FT"];

    #endregion

    #region Constructors

    public GuessesService(ILogger<GuessesService> logger,
                          IApiFootballProvider apiFootballProvider,
                          IGuessesRepository guessesRepository,
                          IUserPointsRepository userPointsRepository,
                          IPointsService pointsService)
    {
        _logger = logger;
        _apiFootballProvider = apiFootballProvider;
        _guessesRepository = guessesRepository;
        _userPointsRepository = userPointsRepository;
        _pointsService = pointsService;
    }

    #endregion

    public async Task ProcessAsync(Fixture fixture, PointSeason pointSeason, Queue<Fixture> fixturesQueue)
    {
        try
        {
            _logger.LogInformation("Retreiving fixture {FixtureId} informations.", fixture.Id);
            var retreivedFixture = await _apiFootballProvider.GetFixture(fixture.Id);

            if (retreivedFixture is null)
            {
                _logger.LogInformation("Fixture {FixtureId} was not found. Breaking operation.", fixture.Id);
                return;
            }

            if (IsNotStarted(retreivedFixture.Fixture?.Status?.Short))
            {
                _logger.LogInformation("Fixture {FixtureId} not started. Breaking operation.", fixture.Id);
                return;
            }

            if (IsUnfinished(retreivedFixture.Fixture?.Status?.Short))
            {
                _logger.LogInformation("Fixture {FixtureId} not finished yet, trying again soon", fixture.Id);
                return;
            }

            var guesses = await _guessesRepository.SelectByFixtureId(fixture.Id);

            if (guesses.Any() is false)
            {
                _logger.LogInformation("No guesses found for fixture {FixtureId}. Breaking operation.", fixture.Id);
                return;
            }

            foreach (var guess in guesses)
            {
                var earnedPoints = await _pointsService.CalculatePoints(guess, retreivedFixture);

                if (earnedPoints > 0)
                {
                    await _userPointsRepository.Insert(new()
                    {
                        UserId = guess.UserId,
                        GameId = guess.GameId,
                        Points = earnedPoints,
                        PointSeasonId = pointSeason.Id
                    });
                }
            }
        }
        catch (Exception ex)
        {
            fixturesQueue.Enqueue(fixture);
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
        }

        static bool IsUnfinished(string? status) => !finishedStatus.Contains(status ?? string.Empty);
        static bool IsNotStarted(string? status) => status?.Equals("NS", StringComparison.OrdinalIgnoreCase) ?? true;
    }
}
