namespace PalpiteFC.Worker.Integrations.Providers.Responses.Entities;

public class Fixture
{
    public int? Id { get; set; }
    public string? Referee { get; set; }
    public string? Timezone { get; set; }
    public DateTimeOffset? Date { get; set; }
    public int? Timestamp { get; set; }
    public Periods? Periods { get; set; }
    public Venue? Venue { get; set; }
    public Status? Status { get; set; }
}

