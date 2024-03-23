using Dapper;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interface;

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

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task Insert(Teams entity)
    {
        throw new NotImplementedException();
    }

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

    public async Task<IEnumerable<Teams>> Select()
        => await _session.Connection.QueryAsync<Teams>("SELECT * FROM teams", null, _session.Transaction);

    public Task<Teams> Select(int id)
    {
        throw new NotImplementedException();
    }

    public Task Update(Teams entity)
    {
        throw new NotImplementedException();
    }
    public Task Update(int id) => throw new NotImplementedException();
    #endregion
}
