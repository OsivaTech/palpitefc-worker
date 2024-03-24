using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;
public interface IUserPointsRepository : IBaseRepository<UserPoints>
{
    Task<IEnumerable<UserPoints>> SelectByUserId(int userId);
}
