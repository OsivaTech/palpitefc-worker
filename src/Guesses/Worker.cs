using PalpiteFC.Api.Domain.Entities.Database;
using PalpiteFC.Worker.Integrations.Interfaces;
using PalpiteFC.Worker.Repository;
using PalpiteFC.Worker.Repository.Interface;
using System.Collections;
using System.Collections.Concurrent;

namespace PalpiteFC.Worker.Guesses;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IFixturesRepository _fixturesRepository;
    private Queue<Fixtures> _queue = new();
    private readonly IApiFootballProvider _apiFootballProvider;
    private readonly IGuessesRepository _guessesRepository;
    private readonly IUserPointsRepository _userPointsRepository;
    public Worker(ILogger<Worker> logger, IApiFootballProvider apiFootballProvider, IGuessesRepository guessesRepository, IUserPointsRepository userPointsRepository, IFixturesRepository fixturesRepository)
    {
        _logger = logger;
        _apiFootballProvider = apiFootballProvider;
        _guessesRepository = guessesRepository;
        _userPointsRepository = userPointsRepository;
        _fixturesRepository = fixturesRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Finding for fixtures: {time}", DateTimeOffset.Now);
                //buscar por jogos da data atual de acordo os dados do banco e a data atual
                var fixtures = await _fixturesRepository.Select(DateTime.Now.Date, DateTime.Now.Date.AddDays(1).AddSeconds(-1));

                _logger.LogInformation("Found {count} fixtures", fixtures.Count());

                fixtures.ToList().ForEach(_queue.Enqueue);

                while (_queue.TryDequeue(out var fixture))
                {
                    if (fixture.Start <= DateTime.Now.AddMinutes(-90))
                    {
                        _ = Task.Run(async () =>
                        {
                            await ProcessarPartida(fixture.Id, stoppingToken);
                        }, stoppingToken);
                    }
                    else
                    {
                        _queue.Enqueue(fixture);
                    }
                }
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while processing fixtures");
            }
            await Task.Delay(600000, stoppingToken);
        }
    }

    private async Task ProcessarPartida(int id, CancellationToken stoppingToken)
    {
        //busca resultado da partida na api de futebol
        _logger.LogInformation("Processing fixture {id}", id);
        while (true)
        {
            //verificar se o resultado da partida já está disponível
            var fixture = await _apiFootballProvider.GetMatch(id);
            if (fixture is null)
            {
                _logger.LogInformation("Fixture {id} not found, enqueuing again", id);
                _queue.Enqueue(await _fixturesRepository.Select(id));
                break;
            }

            if (fixture.Fixture.Status.Short == "FT")
            {
                //buscar por palpites no banco
                var guesses = await _guessesRepository.SelectByFixtureId(id);
                if (guesses is null)
                {
                    _logger.LogInformation("No guesses found for fixture {id}", id);
                    break;
                }
                guesses.ToList().ForEach(guess =>
                {
                    //verificar se os palpites estão corretos
                    var isValidGuess = guess.FirstTeamGol == fixture.Goals!.Home.GetValueOrDefault() && guess.SecondTeamGol == fixture.Goals!.Away.GetValueOrDefault();
                    if (isValidGuess)
                    {
                        _logger.LogInformation("Guess {id} is correct, won 10 points", guess.Id);
                        _userPointsRepository.Insert(new() { GameId = guess.GameId, Points = 10 });
                    }
                });
                break;
            }
            await Task.Delay(60000, stoppingToken);
        }
    }
}
