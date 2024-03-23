using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface ITeamsRepository : IBaseRepository<Teams>
{
    Task InsertOrUpdate(IEnumerable<Teams> list);
}
