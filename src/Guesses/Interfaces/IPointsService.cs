using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Worker.Guesses.DataContracts;
using PalpiteFC.Worker.Integrations.Providers.Responses;

namespace PalpiteFC.Worker.Guesses.Interfaces;

public interface IPointsService
{
    IEnumerable<PointsResult> CalculatePoints(Guess guess, FixtureResponse fixture);
}