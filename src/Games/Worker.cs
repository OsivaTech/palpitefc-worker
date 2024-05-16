using Microsoft.Extensions.Options;
using PalpiteFC.Worker.Games.Interfaces;
using PalpiteFC.Worker.Games.Settings;

namespace PalpiteFC.Worker.Games;

public class Worker : BackgroundService
{
    private readonly IFixturesService _fixturesService;
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<WorkerSettings> _options;

    public Worker(IFixturesService fixturesService, ILogger<Worker> logger, IOptions<WorkerSettings> options)
    {
        _fixturesService = fixturesService;
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (await _fixturesService.TryProcessAsync(stoppingToken) is false)
                {
                    _logger.LogError("Failed to process fixtures, trying again soon...");
                    await Task.Delay(_options.Value.RestartDelay, stoppingToken);

                    continue;
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
