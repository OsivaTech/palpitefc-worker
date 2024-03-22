using PalpiteFC.Api.Domain.Entities.Database;

namespace PalpiteFC.Worker.Repository.Interface;

public interface IGuessesRepository : IBaseRepository<Guesses>
{
    Task<int> InsertAndGetId(Guesses entity);
    Task<IEnumerable<Guesses>> SelectByFixtureId(int id);
    Task<IEnumerable<Guesses>> SelectByUserIdAndGameId(int userId, int gameId);
}
