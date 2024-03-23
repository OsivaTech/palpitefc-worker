using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interface;

public interface IFixturesRepository : IBaseRepository<Fixtures>
{
    Task<int> InsertAndGetId(Fixtures entity);
    Task InsertOrUpdate(IEnumerable<Fixtures> list);
    Task<IEnumerable<Fixtures>> Select(DateTime startDate, DateTime endDate, bool finished);
}