using Dapper;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Repository.Repositories;

public class FixturesRepository : IFixturesRepository
{
    #region Fields

    private readonly DbSession _session;

    #endregion

    #region Constructors

    public FixturesRepository(DbSession session)
    {
        _session = session;
    }

    #endregion

    #region Public Methods

    public async Task Delete(int id)
        => await _session.Connection.ExecuteAsync("DELETE FROM games WHERE id = @id", new { id }, _session.Transaction);

    public Task Insert(Fixtures entity)
        => throw new NotImplementedException();

    public async Task<int> InsertAndGetId(Fixtures entity)
    {
        var query = @"INSERT INTO games (name, championshipId, start, createdAt, updatedAt) VALUES(@name, @championshipId, @start, current_timestamp(3), current_timestamp(3));
                      SELECT LAST_INSERT_ID() as id;";

        return await _session.Connection.QuerySingleAsync<int>(query, new { entity.Name, entity.ChampionshipId, entity.Start }, _session.Transaction);
    }

    public async Task InsertOrUpdate(IEnumerable<Fixtures> list)
    {
        var query = @"INSERT INTO games (
	                      id
	                      ,name
	                      ,championshipId
	                      ,start
	                      ,finished
	                      ,createdAt
	                      ,updatedAt
	                      )
                      VALUES (
	                      @id
	                      ,@name
	                      ,@championshipId
	                      ,@start
	                      ,@finished
	                      ,current_timestamp(3)
	                      ,current_timestamp(3)
	                      ) ON DUPLICATE KEY
                      UPDATE name = @name
	                      ,championshipId = @championshipId
	                      ,start = @start
	                      ,finished = @finished
	                      ,updatedAt = current_timestamp(3);";

        await _session.Connection.ExecuteAsync(query, list, _session.Transaction);
    }

    public async Task<IEnumerable<Fixtures>> Select()
        => await _session.Connection.QueryAsync<Fixtures>("SELECT * FROM games", null, _session.Transaction);

    public async Task<Fixtures> Select(int id)
        => await _session.Connection.QuerySingleAsync<Fixtures>("SELECT * FROM games WHERE id = @id", new { id }, _session.Transaction);
    
    public async Task<IEnumerable<Fixtures>> Select(DateTime startDate, DateTime endDate, bool finished)
    {
        var query = @"SELECT * FROM games
                        WHERE start BETWEEN @startDate AND @endDate AND finished = @finished";

        return await _session.Connection.QueryAsync<Fixtures>(query, new { startDate, endDate, finished }, _session.Transaction);
    }

    public async Task Update(Fixtures entity)
        => await _session.Connection.ExecuteAsync("UPDATE games SET name = @name, championshipId = @championshipId, start = @start, finished = @finished, updatedAt = current_timestamp(3) WHERE id = @id",
            new { entity.Name, entity.ChampionshipId, entity.Start, entity.Finished, entity.Id }, _session.Transaction);

    public Task Update(int id) => throw new NotImplementedException();

    #endregion
}