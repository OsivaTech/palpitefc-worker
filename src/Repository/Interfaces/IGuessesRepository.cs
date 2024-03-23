using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface IGuessesRepository : IBaseRepository<Guesses>
{
    Task<int> InsertAndGetId(Guesses entity);
    Task<IEnumerable<Guesses>> SelectByFixtureId(int id);
    Task<IEnumerable<Guesses>> SelectByUserIdAndGameId(int userId, int gameId);
}
