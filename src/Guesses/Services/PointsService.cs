using Microsoft.Extensions.Options;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Settings;
using PalpiteFC.Worker.Integrations.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Guesses.Services;

public class PointsService : IPointsService
{
    private readonly IUserPointsRepository _userPointsRepository;
    private readonly IOptions<PointsSettings> _pointsSettings;
    private readonly ILogger<PointsService> _logger;

    public PointsService(IOptions<PointsSettings> pointsSettings, IUserPointsRepository userPointsRepository, ILogger<PointsService> logger)
    {
        _pointsSettings = pointsSettings;
        _userPointsRepository = userPointsRepository;
        _logger = logger;
    }

    public async Task<int> CalculatePoints(Repository.Entities.Guesses guesses, Match fixture)
    {
        _logger.LogInformation("Calculating points for user {id} and guess {guess}", guesses.UserId, guesses.Id);

        var isValidGuess = guesses.FirstTeamGol == fixture.Goals?.Home && guesses.SecondTeamGol == fixture.Goals?.Away;

        if (isValidGuess is false)
        {
            _logger.LogInformation("Guess {Id} is wrong. User {UserId} did not win any points", guesses.Id, guesses.UserId);
            return 0;
        }

        var existingPoint = await _userPointsRepository.SelectByUserId(guesses.UserId);

        if (existingPoint.Any(w => w.GameId == guesses.GameId))
        {
            _logger.LogInformation("User {userId} has already points for game {gameId}", guesses.UserId, guesses.GameId);
            return 0;
        }

        var earnedPoints = _pointsSettings.Value.HitResult;

        _logger.LogInformation("Guess {Id} is correct. User {UserId} won {Points} points", guesses.Id, guesses.UserId, earnedPoints);

        return earnedPoints;
    }
}
