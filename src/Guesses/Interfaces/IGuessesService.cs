using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Guesses.Interfaces;

public interface IGuessesService
{
    Task ProcessAsync(Fixtures fixture, PointSeasons pointSeason, Queue<Fixtures> fixturesQueue);
}