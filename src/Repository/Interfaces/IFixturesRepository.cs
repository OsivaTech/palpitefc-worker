using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface IFixturesRepository
{
    Task<IEnumerable<Fixtures>> Select(DateTime startDate, DateTime endDate);
    Task InsertOrUpdate(IEnumerable<Fixtures> list);
}