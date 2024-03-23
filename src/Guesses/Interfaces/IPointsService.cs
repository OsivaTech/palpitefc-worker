using PalpiteFC.Worker.Integrations.Entities;

namespace PalpiteFC.Worker.Guesses.Interfaces;

public interface IPointsService
{
    Task<int> CalculatePoints(Repository.Entities.Guesses guesses, Match fixture, int points);
}
