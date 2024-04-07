using Dapper;
using PalpiteFC.Worker.Repository.Connection;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Repository.Repositories;

public class GuessesRepository : IGuessesRepository
{
    #region Fields

    private readonly DbSession _session;

    #endregion

    #region Constructors

    public GuessesRepository(DbSession session)
    {
        _session = session;
    }

    #endregion

    #region Public Methods

    public async Task<IEnumerable<Guesses>> SelectByUserIdAndGameId(int userId, int gameId) 
        => await _session.Connection.QueryAsync<Guesses>(@"SELECT * FROM palpitations WHERE userId = @userId AND gameId = @gameId", new { userId, gameId }, _session.Transaction);

    public async Task<IEnumerable<Guesses>> SelectByFixtureId(int id)
        => await _session.Connection.QueryAsync<Guesses>("SELECT * FROM palpitations WHERE gameId = @id", new { id }, _session.Transaction);

    #endregion
}
