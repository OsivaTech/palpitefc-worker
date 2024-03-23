using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface ITeamsGamesRepository : IBaseRepository<TeamsGame>
{
    Task InsertOrUpdate(IEnumerable<TeamsGame> list);
}
