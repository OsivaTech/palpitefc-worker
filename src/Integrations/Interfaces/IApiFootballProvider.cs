using PalpiteFC.Worker.Integrations.Entities;

namespace PalpiteFC.Worker.Integrations.Interfaces;

public interface IApiFootballProvider
{
    Task<IEnumerable<Match>> GetMatchesByLeagueId(int leagueId, int season, string fromDate, string toDate);
    Task<Match> GetMatch(int fixtureId);

}