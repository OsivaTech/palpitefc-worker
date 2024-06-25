using PalpiteFC.Libraries.Persistence.Abstractions.Connection;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Integrations.Interfaces;
using System.Text.Json;

namespace PalpiteFC.Worker.Guesses.Services;

public class GuessesService : IGuessesService
{
    #region Fields

    private readonly ILogger<GuessesService> _logger;
    private readonly IApiFootballProvider _apiFootballProvider;
    private readonly IGuessesRepository _guessesRepository;
    private readonly IUserPointsRepository _userPointsRepository;
    private readonly IPointsService _pointsService;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly string[] finishedStatus = ["PEN", "AET", "FT"];
    private static readonly string[] unprocessableStatus = ["TBD", "NS", "PST", "CANC", "ABD", "AWD", "WO"];

    #endregion

    #region Constructors

    public GuessesService(ILogger<GuessesService> logger,
                          IApiFootballProvider apiFootballProvider,
                          IGuessesRepository guessesRepository,
                          IUserPointsRepository userPointsRepository,
                          IPointsService pointsService,
                          IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _apiFootballProvider = apiFootballProvider;
        _guessesRepository = guessesRepository;
        _userPointsRepository = userPointsRepository;
        _pointsService = pointsService;
        _unitOfWork = unitOfWork;
    }

    #endregion

    public async Task<bool> TryProcessAsync(Fixture fixture)
    {
        try
        {
            _logger.LogInformation("Retreiving fixture {FixtureId} informations.", fixture.Id);
            var retreivedFixture = await _apiFootballProvider.GetFixture(fixture.ExternalId);

            if (retreivedFixture is null)
            {
                _logger.LogWarning("Fixture {FixtureId} was not found. Breaking operation.", fixture.Id);
                return true;
            }

            if (IsUnprocessableStatus(retreivedFixture.Fixture?.Status?.Short))
            {
                _logger.LogWarning("Fixture {FixtureId} has an unprocessable status: {Status}. Breaking operation.", fixture.Id, retreivedFixture.Fixture?.Status?.Long);
                return true;
            }

            if (IsUnfinished(retreivedFixture.Fixture?.Status?.Short))
            {
                _logger.LogInformation("Fixture {FixtureId} not finished yet, trying again soon", fixture.Id);
                return false;
            }

            var guesses = await _guessesRepository.SelectByFixtureId(fixture.Id);

            if (guesses.Any() is false)
            {
                _logger.LogWarning("No guesses found for fixture {FixtureId}. Breaking operation.", fixture.Id);
                return true;
            }

            foreach (var guess in guesses)
            {
                var pointExists = await _userPointsRepository.CheckExistingPoint(fixture.Id, guess.UserId);

                if (pointExists)
                {
                    _logger.LogInformation("User {UserId} has already points for game {FixtureId}", guess.UserId, guess.FixtureId);
                    continue;
                }

                _logger.LogInformation("Calculating points for user {UserId} and guess {GuessId}", guess.UserId, guess.Id);

                var earnedPoints = _pointsService.CalculatePoints(guess, retreivedFixture);

                _logger.LogInformation("Guess {GuessId} for user {UserId} results: {Results}", guess.Id, guess.UserId, JsonSerializer.Serialize(earnedPoints));

                _unitOfWork.BeginTransaction();

                foreach (var item in earnedPoints)
                {
                    await _userPointsRepository.Insert(new UserPoint()
                    {
                        UserId = guess.UserId,
                        FixtureId = guess.FixtureId,
                        Points = item.Points,
                        Type = item.Type.ToString(),
                        GuessId = guess.Id
                    });
                }

                _unitOfWork.Commit();
            }

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();

            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            return false;
        }

        static bool IsUnfinished(string? status) => !finishedStatus.Contains(status ?? string.Empty);
        static bool IsUnprocessableStatus(string? status) => unprocessableStatus.Contains(status ?? string.Empty);
    }
}
