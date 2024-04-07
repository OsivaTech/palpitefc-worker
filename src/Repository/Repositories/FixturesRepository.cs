using Dapper;
using PalpiteFC.Worker.Repository.Connection;
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

    public async Task<IEnumerable<Fixtures>> Select(DateTime startDate, DateTime endDate)
    {
        var query = @"SELECT * FROM games
                        WHERE start BETWEEN @startDate AND @endDate";

        return await _session.Connection.QueryAsync<Fixtures>(query, new { startDate, endDate }, _session.Transaction);
    }

    #endregion
}