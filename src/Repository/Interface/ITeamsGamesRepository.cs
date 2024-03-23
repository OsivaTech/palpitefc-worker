using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interface;

public interface ITeamsGamesRepository : IBaseRepository<TeamsGame>
{
    Task InsertOrUpdate(IEnumerable<TeamsGame> list);
}
