using Microsoft.Extensions.Options;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Settings;
using PalpiteFC.Worker.Integrations.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Guesses.Services;
public class PointsService : IPointsService
{
    private readonly IGuessesRepository _guessesRepository;
    private readonly IUserPointsRepository _userPointsRepository;
    private readonly ILogger<PointsService> _logger;
    private readonly IOptions<PointsSettings> _pointsSettings;
    public PointsService(IGuessesRepository guessesRepository, IUserPointsRepository userPointsRepository, ILogger<PointsService> logger, IOptions<PointsSettings> pointsSettings)
    {
        _guessesRepository = guessesRepository;
        _userPointsRepository = userPointsRepository;
        _logger = logger;
        _pointsSettings = pointsSettings;
    }

    public async Task<int> CalculatePoints(Repository.Entities.Guesses guess, Match fixture, int points)
    {
        var hasVoted = await _guessesRepository.SelectByUserIdAndGameId(guess.UserId, guess.GameId);
        if (hasVoted.Any())
        {
            _logger.LogInformation("User {userId} has already voted for game {gameId}", guess.UserId, guess.GameId);
            return 0;
        }
        _logger.LogInformation("Calculating points for user {id} and guess {guess}", guess.UserId, guess.Id);
        var isValidGuess = guess.FirstTeamGol == fixture.Goals!.Home.GetValueOrDefault() && guess.SecondTeamGol == fixture.Goals!.Away.GetValueOrDefault();
        if (isValidGuess)
        {
            _logger.LogInformation("Guess {id} is correct, won 10 points", guess.Id);
            points = _pointsSettings.Value.HitResult;
        }

        return points;
    }
}
