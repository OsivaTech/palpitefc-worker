using PalpiteFC.Libraries.Persistence.Abstractions.Entities;

namespace PalpiteFC.Worker.Guesses.Interfaces;

public interface IGuessesService
{
    Task<bool> TryProcessAsync(Fixture fixture, PointSeason pointSeason);
}