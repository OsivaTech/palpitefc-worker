using Dapper;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interface;

namespace PalpiteFC.Worker.Repository.Repositories;

public class TeamsGameRepository : ITeamsGamesRepository
{
    #region Fields

    private readonly DbSession _session;

    #endregion

    #region Constructors

    public TeamsGameRepository(DbSession session)
    {
        _session = session;
    }

    #endregion

    #region Public Methods

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task Insert(TeamsGame obj)
    {
        throw new NotImplementedException();
    }

    public async Task InsertOrUpdate(IEnumerable<TeamsGame> list)
    {
        var query = @"INSERT INTO teamsGame (
                    	gol
                    	,teamId
                    	,gameId
                    	,createdAt
                    	,updatedAt
                    	)
                    VALUES (
                    	@gol
                    	,@teamId
                    	,@gameId
                    	,current_timestamp(3)
                    	,current_timestamp(3)
                    	) ON DUPLICATE KEY
                    UPDATE gol = @gol
                    	,teamId = @teamId
                    	,gameId = @gameId
                    	,updatedAt = current_timestamp(3);";
        
        await _session.Connection.ExecuteAsync(query, list, _session.Transaction);
    }

    public async Task<IEnumerable<TeamsGame>> Select()
        => await _session.Connection.QueryAsync<TeamsGame>("SELECT * FROM teamsGame", null, _session.Transaction);

    public Task<TeamsGame> Select(int id)
    {
        throw new NotImplementedException();
    }

    public Task Update(TeamsGame obj)
    {
        throw new NotImplementedException();
    }
    public Task Update(int id) => throw new NotImplementedException();
    #endregion
}
