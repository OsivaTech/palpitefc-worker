using Dapper;
using PalpiteFC.Worker.Repository.Connection;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Repository.Repositories;

internal class PointSeasonsRepository : IPointSeasonsRepository
{
    #region Fields

    private readonly DbSession _session;

    #endregion

    #region Constructors

    public PointSeasonsRepository(DbSession session)
    {
        _session = session;
    }

    #endregion

    #region Public Methods

    public async Task<PointSeasons> SelectCurrent()
    {
        var query = "SELECT * FROM pointSeasons WHERE current_timestamp(3) BETWEEN startDate AND endDate";

        return (await _session.Connection.QueryFirstOrDefaultAsync<PointSeasons>(query, null, _session.Transaction))!;
    }

    #endregion
}
