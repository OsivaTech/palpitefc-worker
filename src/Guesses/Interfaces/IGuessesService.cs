using PalpiteFC.Libraries.Persistence.Abstractions.Entities;

namespace PalpiteFC.Worker.Guesses.Interfaces;

public interface IGuessesService
{
    Task ProcessAsync(Fixture fixture, PointSeason pointSeason, Queue<Fixture> fixturesQueue);
}