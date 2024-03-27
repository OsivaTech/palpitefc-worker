using PalpiteFC.Worker.Integrations.Entities;
using PalpiteFC.Worker.Integrations.Requests;

namespace PalpiteFC.Worker.Integrations.Interfaces;

public interface IApiFootballProvider
{
    Task<IEnumerable<Match>> GetFixtures(FixturesRequest request);
    Task<Match> GetFixture(int fixtureId);
}