using PalpiteFC.Api.Domain.Entities.Database;

namespace PalpiteFC.Worker.Repository.Interface;

public interface IFixturesRepository : IBaseRepository<Fixtures>
{
    Task<int> InsertAndGetId(Fixtures entity);
    Task<IEnumerable<Fixtures>> Select(DateTime startDate, DateTime endDate, bool finished);
}
