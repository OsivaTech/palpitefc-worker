using Dapper;
using PalpiteFC.Worker.Repository.Connection;
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

    public async Task<IEnumerable<Leagues>> Select()
    => await _session.Connection.QueryAsync<Leagues>("SELECT * FROM championships", null, _session.Transaction);

    public async Task<int> InsertAndGetId(Leagues entity)
    {
        var query = @"INSERT INTO championships (name, createdAt, updatedAt) VALUES(@name, current_timestamp(3), current_timestamp(3));
                      SELECT LAST_INSERT_ID() as id;";

        return await _session.Connection.QuerySingleAsync<int>(query, new { entity.Name }, _session.Transaction);
    }

    #endregion
}
