using Dapper;
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

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Guesses>> SelectByUserIdAndGameId(int userId, int gameId)
    {
        var query = @"SELECT * FROM palpitations
                        WHERE userId = @userId AND gameId = @gameId";

        return await _session.Connection.QueryAsync<Guesses>(query, new { userId, gameId }, _session.Transaction);
    }

    public async Task Insert(Guesses entity)
    {
        var query = @"INSERT INTO palpitations
                        (firstTeamId, firstTeamGol, secondTeamId, secondTeamGol, userId, gameId, createdAt, updatedAt)
                        VALUES(@firstTeamId, @firstTeamGol, @secondTeamId, @secondTeamGol, @userId, @gameId, current_timestamp(3), current_timestamp(3));";

        await _session.Connection.ExecuteAsync(query, entity, _session.Transaction);
    }

    public async Task<int> InsertAndGetId(Guesses entity)
    {
        var query = @"INSERT INTO palpitations
                        (firstTeamId, firstTeamGol, secondTeamId, secondTeamGol, userId, gameId, createdAt, updatedAt)
                        VALUES(@firstTeamId, @firstTeamGol, @secondTeamId, @secondTeamGol, @userId, @gameId, current_timestamp(3), current_timestamp(3));
                      SELECT LAST_INSERT_ID() as id;";

        return await _session.Connection.QuerySingleAsync<int>(query, entity, _session.Transaction);
    }

    public Task<IEnumerable<Guesses>> Select()
    {
        throw new NotImplementedException();
    }
    
    public async Task<Guesses> Select(int id)
        => await _session.Connection.QuerySingleAsync<Guesses>("SELECT * FROM palpitations WHERE id = @id", new { id }, _session.Transaction);
    public async Task<IEnumerable<Guesses>> SelectByFixtureId(int id)
        => await _session.Connection.QueryAsync<Guesses>("SELECT * FROM palpitations WHERE gameId = @id", new { id }, _session.Transaction);

    public Task Update(Guesses obj)
    {
        throw new NotImplementedException();
    }

    #endregion
}
