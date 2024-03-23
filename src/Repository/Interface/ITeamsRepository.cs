using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interface;

public interface ITeamsRepository : IBaseRepository<Teams>
{
    Task InsertOrUpdate(IEnumerable<Teams> list);
}
