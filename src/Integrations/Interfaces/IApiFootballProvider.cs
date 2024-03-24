using PalpiteFC.Worker.Integrations.Entities;

namespace PalpiteFC.Worker.Integrations.Interfaces;

public interface IApiFootballProvider
{
    Task<IEnumerable<Match>> GetFixtures(int leagueId, int season, string fromDate, string toDate);
    Task<Match> GetFixture(int fixtureId);

}