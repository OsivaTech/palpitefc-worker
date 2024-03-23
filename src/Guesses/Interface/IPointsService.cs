using PalpiteFC.Api.Domain.Entities.ApiFootball;

namespace PalpiteFC.Worker.Guesses.Interface;
public interface IPointsService
{
    Task<int> CalculatePoints(Api.Domain.Entities.Database.Guesses guesses, Match fixture, int points);
}
