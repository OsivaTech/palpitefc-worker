using Dapper;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Repository.Repositories;

public class LeaguesRepository : ILeaguesRepository
{
    #region Fields

    private readonly DbSession _session;

    #endregion

    #region Constructors

    public LeaguesRepository(DbSession session)
    {
        _session = session;
    }

    #endregion

    #region Public Methods

    public async Task Delete(int id)
        => await _session.Connection.ExecuteAsync("DELETE FROM championships WHERE id = @id", new { id }, _session.Transaction);

    public Task Insert(Championships entity)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InsertAndGetId(Championships entity)
    {
        var query = @"INSERT INTO championships (name, createdAt, updatedAt) VALUES(@name, current_timestamp(3), current_timestamp(3));
                      SELECT LAST_INSERT_ID() as id;";

        return await _session.Connection.QuerySingleAsync<int>(query, new { entity.Name }, _session.Transaction);
    }

    public async Task<IEnumerable<Championships>> Select()
        => await _session.Connection.QueryAsync<Championships>("SELECT * FROM championships", null, _session.Transaction);

    public async Task<Championships> Select(int id)
        => await _session.Connection.QuerySingleAsync<Championships>("SELECT * FROM championships WHERE id = @id", new { id }, _session.Transaction);

    public async Task Update(Championships entity)
        => await _session.Connection.ExecuteAsync("UPDATE championships SET name = @name, updatedAt = current_timestamp(3) WHERE id = @id", new { entity.Name, entity.Id }, _session.Transaction);

    public Task Update(int id)
    {
        throw new NotImplementedException();
    }

    #endregion
}
