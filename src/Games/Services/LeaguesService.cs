using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.Persistence.Abstractions.Repositories;
using PalpiteFC.Worker.Games.Interfaces;
using PalpiteFC.Worker.Integrations.Interfaces;

namespace PalpiteFC.Worker.Games.Services;

public class LeaguesService : ILeaguesService
{
    private readonly ILeaguesRepository _repository;
    private readonly IApiFootballProvider _apiFootballProvider;
    private readonly ILogger<LeaguesService> _logger;

    public LeaguesService(ILeaguesRepository leaguesRepository, IApiFootballProvider apiFootballProvider, ILogger<LeaguesService> logger)
    {
        _repository = leaguesRepository;
        _apiFootballProvider = apiFootballProvider;
        _logger = logger;
    }

    public async Task<bool> TryProcessAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting loading leagues");
        _logger.LogInformation("Retreiving leagues from ApiFootball");

        var leagues = await _apiFootballProvider.GetLeagues(new() { Season = DateTime.Now.Year });

        _logger.LogInformation("Retreived {LeaguesCount} leagues", leagues.Count());

        var entity = leagues.Select(l => new League
        {
            ExternalId = l.League?.Id ?? 0,
            DataSourceId = 1,
            Name = l.League?.Name,
            Image = l.League?.Logo
        }).OrderBy(x => x.ExternalId);

        _logger.LogInformation("Saving data on database");

        await _repository.InsertOrUpdate(entity);

        _logger.LogInformation("Done!");

        return true;
    }
}