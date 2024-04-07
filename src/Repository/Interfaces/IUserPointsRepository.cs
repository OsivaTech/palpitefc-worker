using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;
public interface IUserPointsRepository
{
    Task Insert(UserPoints entity);
    Task<IEnumerable<UserPoints>> SelectByUserId(int userId);
}
