namespace PalpiteFC.Worker.Games.Interfaces;

public interface ILeaguesService
{
    Task<bool> TryProcessAsync(CancellationToken stoppingToken);
}