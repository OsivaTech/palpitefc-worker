using PalpiteFC.Worker.Integrations.Providers.Responses;
using PalpiteFC.Worker.Integrations.Requests;

namespace PalpiteFC.Worker.Integrations.Interfaces;

public interface IApiFootballProvider
{
    Task<IEnumerable<FixtureResponse>> GetFixtures(FixturesRequest request);
    Task<FixtureResponse> GetFixture(int fixtureId);
}