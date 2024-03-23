using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface ILeaguesRepository : IBaseRepository<Championships>
{
    Task<int> InsertAndGetId(Championships entity);
}

