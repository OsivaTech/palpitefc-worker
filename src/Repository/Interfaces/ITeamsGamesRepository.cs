using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface ITeamsGamesRepository
{
    Task InsertOrUpdate(IEnumerable<TeamsGame> list);
}
