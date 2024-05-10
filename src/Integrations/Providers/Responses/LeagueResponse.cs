using PalpiteFC.Worker.Integrations.Providers.Responses.Entities;

namespace PalpiteFC.Worker.Integrations.Providers.Responses;

public class LeagueResponse
{
    public League? League { get; set; }
    public Country? Country { get; set; }
    public Season[]? Seasons { get; set; }
}