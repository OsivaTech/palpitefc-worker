using PalpiteFC.Api.Domain.Entities.ApiFootball;

namespace PalpiteFC.Worker.Integrations.Interfaces;

public interface IApiFootballProvider
{
    Task<IEnumerable<Match>> GetMatchesByLeagueId(string leagueId, string fromDate, string toDate);
    Task<Match> GetMatch(int fixtureId);

}