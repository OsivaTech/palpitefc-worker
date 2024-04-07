using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface IGuessesRepository
{
    Task<IEnumerable<Guesses>> SelectByFixtureId(int id);
}
