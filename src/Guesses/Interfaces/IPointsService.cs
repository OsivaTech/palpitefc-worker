using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using Match = PalpiteFC.Worker.Integrations.Providers.Responses.Match;

namespace PalpiteFC.Worker.Guesses.Interfaces;

public interface IPointsService
{
    Task<int> CalculatePoints(Guess guess, Match match);
}
