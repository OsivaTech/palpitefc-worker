using Dapper;
using PalpiteFC.Worker.Repository.Entities;
using PalpiteFC.Worker.Repository.Interfaces;

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

    public async Task<IEnumerable<UserPoints>> SelectByUserId(int userId)
        => await _dbSession.Connection.QueryAsync<UserPoints>("SELECT * FROM userPoints WHERE userId = @userId", new { userId }, _dbSession.Transaction);

    public Task Update(UserPoints entity)
    {
        throw new NotImplementedException();
    }
}
