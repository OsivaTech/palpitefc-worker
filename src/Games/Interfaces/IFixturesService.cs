namespace PalpiteFC.Worker.Games.Interfaces;

public interface IFixturesService
{
    Task<bool> TryProcessAsync(CancellationToken stoppingToken);
}