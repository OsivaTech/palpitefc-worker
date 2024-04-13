using Mapster;
using PalpiteFC.DataContracts.MessageTypes;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Persistence.Interfaces;

namespace PalpiteFC.Worker.Persistence.Services;
public class GuessService : IGuessService
{
    private readonly ILogger<GuessService> _logger;
    private readonly IGuessesRepository _repository;
    public GuessService(ILogger<GuessService> logger, IGuessesRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    public async Task ProcessMessage(GuessMessage guess)
    {
        try
        {
            //var guess = JsonSerializer.Deserialize<Guess>(message);
            if (guess == null)
            {
                _logger.LogError("Invalid message: {Message}", guess);
                return;
            }

            // valida se já existe um palpite para o mesmo jogo e usuário
            var existingGuess = await _repository.SelectByUserIdAndGameId(guess.UserId, guess.GameId);
            if (existingGuess.Any())
            {
                _logger.LogWarning("Guess already exists for user {UserId} and game {GameId}", guess.UserId, guess.GameId);
                return;
            }

            _logger.LogInformation("Inserting guess to user {UserId} and game {GameId}", guess.UserId, guess.GameId);
            // grava mensagem no banco
            await _repository.Insert(guess.Adapt<Guess>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", guess);
        }
    }
}
