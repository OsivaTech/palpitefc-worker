using Microsoft.Extensions.Options;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Services;
using PalpiteFC.Worker.Guesses.Settings;

namespace PalpiteFC.Worker.Guesses;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<WorkerSettings> _options;
    private readonly IFixturesRepository _fixturesRepository;
    private readonly IPointSeasonsRepository _pointSeasonsRepository;
    private readonly IGuessesService _guessesProcessorService;

    public Worker(ILogger<Worker> logger,
                  IOptions<WorkerSettings> options,
                  IFixturesRepository fixturesRepository,
                  IPointSeasonsRepository pointSeasonsRepository,
                  IGuessesService guessesProcessorService)
    {
        _logger = logger;
        _options = options;
        _fixturesRepository = fixturesRepository;
        _pointSeasonsRepository = pointSeasonsRepository;
        _guessesProcessorService = guessesProcessorService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var queue = new Queue<QueueObject<Fixture>>();

                var pointSeason = await _pointSeasonsRepository.SelectCurrent();

                if (pointSeason is null)
                {
                    _logger.LogWarning("No PointSeason was found for the current period. Breaking operation.");
                    await Task.Delay(_options.Value.RestartDelay, stoppingToken);

                    continue;
                }

                _logger.LogInformation("Current PointSeason is {PointSeasonId}.", pointSeason.Id);

                var fixtures = await _fixturesRepository.Select(DateTime.Now.Date, DateTime.Now.Date.AddDays(1).AddTicks(-1));
                _logger.LogInformation("Found {FixtureCount} fixtures.", fixtures.Count());

                fixtures.ToList().ForEach(x => queue.Enqueue(new QueueObject<Fixture>(x)));

                while (queue.TryDequeue(out var fixture))
                {
                    if (fixture.Data.Start >= DateTime.Now.AddMinutes(_options.Value.ProcessGuessesAfter.TotalMinutes * -1))
                    {
                        queue.Enqueue(fixture);
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                        continue;
                    }

                    if (fixture.LastTry >= DateTime.Now.AddMinutes(_options.Value.ReprocessAfter.TotalMinutes * -1))
                    {
                        queue.Enqueue(fixture);
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                        continue;
                    }

                    var result = await _guessesProcessorService.TryProcessAsync(fixture.Data, pointSeason);

                    if (result is false)
                    {
                        fixture.Tries++;
                        fixture.LastTry = DateTime.Now;

                        queue.Enqueue(fixture);
                    }
                }

                _logger.LogInformation("Service finished processing! Starting again in {LoopDelay}.", _options.Value.LoopDelay);
                await Task.Delay(_options.Value.LoopDelay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                _logger.LogInformation("Restarting service in {Time}", _options.Value.RestartDelay);

                await Task.Delay(_options.Value.RestartDelay, stoppingToken);
            }
        }
    }
}
