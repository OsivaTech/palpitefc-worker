using Microsoft.Extensions.Options;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Settings;
using Match = PalpiteFC.Worker.Integrations.Providers.Responses.Match;

namespace PalpiteFC.Worker.Guesses.Services;

public class PointsService : IPointsService
{
    private readonly IUserPointsRepository _userPointsRepository;
    private readonly IOptions<WorkerSettings> _workerSettings;
    private readonly ILogger<PointsService> _logger;

    public PointsService(IOptions<WorkerSettings> workerSettings, IUserPointsRepository userPointsRepository, ILogger<PointsService> logger)
    {
        _workerSettings = workerSettings;
        _userPointsRepository = userPointsRepository;
        _logger = logger;
    }

    public async Task<int> CalculatePoints(Guess guess, Match match)
    {
        _logger.LogInformation("Calculating points for user {UserId} and guess {GuessId}", guess.UserId, guess.Id);

        var isValidGuess = guess.FirstTeamGol == match.Goals?.Home && guess.SecondTeamGol == match.Goals?.Away;

        if (isValidGuess is false)
        {
            _logger.LogInformation("Guess {GuessId} is wrong. User {UserId} did not win any points", guess.Id, guess.UserId);
            return 0;
        }

        var existingPoint = await _userPointsRepository.SelectByUserId(guess.UserId);

        if (existingPoint.Any(w => w.GameId == guess.GameId))
        {
            _logger.LogInformation("User {UserId} has already points for game {GameId}", guess.UserId, guess.GameId);
            return 0;
        }

        var earnedPoints = _workerSettings.Value.Points!.HitResult;

        _logger.LogInformation("Guess {GuessId} is correct. User {UserId} won {Points} points", guess.Id, guess.UserId, earnedPoints);

        return earnedPoints;
    }
}
