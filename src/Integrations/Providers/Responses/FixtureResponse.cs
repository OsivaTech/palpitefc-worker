using PalpiteFC.Worker.Integrations.Providers.Responses.Entities;

namespace PalpiteFC.Worker.Integrations.Providers.Responses;

public class FixtureResponse
{
    public Fixture? Fixture { get; set; }
    public League? League { get; set; }
    public TeamsPlaying? Teams { get; set; }
    public Goals? Goals { get; set; }
    public Score? Score { get; set; }
}