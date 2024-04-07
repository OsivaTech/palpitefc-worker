using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface IPointSeasonsRepository
{
    Task<PointSeasons> SelectCurrent();
}