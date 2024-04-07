using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface ITeamsRepository
{
    Task InsertOrUpdate(IEnumerable<Teams> list);
}
