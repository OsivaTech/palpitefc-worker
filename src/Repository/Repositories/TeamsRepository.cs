using Dapper;
using PalpiteFC.Worker.Repository.Connection;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Repository.Repositories;

public class TeamsRepository : ITeamsRepository
{
    #region Fields

    private readonly DbSession _session;

    #endregion

    #region Constructors

    public TeamsRepository(DbSession session)
    {
        _session = session;
    }

    #endregion

    #region Public Methods

    public async Task InsertOrUpdate(IEnumerable<Teams> list)
    {
        var query = @"INSERT INTO teams (
	                      id
	                      ,name
	                      ,image
	                      ,createdAt
	                      ,updatedAt
	                      )
                      VALUES (
	                      @id
	                      ,@name
	                      ,@image
	                      ,current_timestamp(3)
	                      ,current_timestamp(3)
	                      ) ON DUPLICATE KEY
                      UPDATE name = @name
	                      ,image = @image
	                      ,updatedAt = current_timestamp(3);";

        await _session.Connection.ExecuteAsync(query, list, _session.Transaction);
    }

    #endregion
}
