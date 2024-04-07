using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface ILeaguesRepository
{
    Task<IEnumerable<Leagues>> Select();
    Task<int> InsertAndGetId(Leagues entity);
}

