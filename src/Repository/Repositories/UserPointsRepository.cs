using Dapper;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interface;

namespace PalpiteFC.Worker.Repository.Repositories;
internal class UserPointsRepository : IUserPointsRepository
{
    private readonly DbSession _dbSession;

    public UserPointsRepository(DbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task Insert(UserPoints entity)
    {
        await _dbSession.Connection.ExecuteAsync("INSERT INTO userPoints (userId, gameId, points, createdAt, updatedAt) " +
            "VALUES (@userId, @gameId, @points, current_timestamp(3), current_timestamp(3));", entity, _dbSession.Transaction);
    }

    public Task<IEnumerable<UserPoints>> Select()
    {
        throw new NotImplementedException();
    }

    public Task<UserPoints> Select(int id)
    {
        throw new NotImplementedException();
    }

    public Task Update(UserPoints entity)
    {
        throw new NotImplementedException();
    }
}
