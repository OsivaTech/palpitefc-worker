namespace PalpiteFC.Worker.Integrations.Providers.Responses.Entities;

public class Coverage
{
    public CoverageFixtures? Fixtures { get; set; }
    public bool Standings { get; set; }
    public bool Players { get; set; }
    public bool Top_scorers { get; set; }
    public bool Top_assists { get; set; }
    public bool Top_cards { get; set; }
    public bool Injuries { get; set; }
    public bool Predictions { get; set; }
    public bool Odds { get; set; }
}

public class CoverageFixtures
{
    public bool Events { get; set; }
    public bool Lineups { get; set; }
    public bool Statistics_fixtures { get; set; }
    public bool Statistics_players { get; set; }
}