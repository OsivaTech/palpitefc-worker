using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Worker.Integrations.Providers.Responses;

namespace PalpiteFC.Worker.Guesses.Interfaces;

public interface IPointsService
{
    int CalculatePoints(Guess guess, FixtureResponse fixture);
}
