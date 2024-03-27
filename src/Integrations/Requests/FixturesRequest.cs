namespace PalpiteFC.Worker.Integrations.Requests;

public class FixturesRequest
{
    public int LeagueId { get; set; }
    public int Season { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? Timezone { get; set; }
}
