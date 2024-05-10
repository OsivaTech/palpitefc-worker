using Microsoft.Extensions.Options;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Settings;
using PalpiteFC.Worker.Integrations.Providers.Responses;

namespace PalpiteFC.Worker.Guesses.Services;

public class PointsService : IPointsService
{
    private readonly IOptions<WorkerSettings> _workerSettings;
    private readonly ILogger<PointsService> _logger;

    public PointsService(IOptions<WorkerSettings> workerSettings, ILogger<PointsService> logger)
    {
        _workerSettings = workerSettings;
        _logger = logger;
    }

    public int CalculatePoints(Guess guess, FixtureResponse fixture)
    {
        _logger.LogInformation("Calculating points for user {UserId} and guess {GuessId}", guess.UserId, guess.Id);

        var earnedPoints = 0;
        var isValidGuess = guess.HomeGoals == fixture.Goals?.Home && guess.AwayGoals == fixture.Goals?.Away;

        if (isValidGuess)
        {
            earnedPoints = _workerSettings.Value.Points!.HitResult;
        }

        _logger.LogInformation("Guess {GuessId} is {GuessIs}. User {UserId} won {Points} points", guess.Id, isValidGuess ? "correct" : "wrong", guess.UserId, earnedPoints);

        return earnedPoints;
    }
}
