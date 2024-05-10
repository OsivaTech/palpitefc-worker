namespace PalpiteFC.Worker.Integrations.Providers.Responses.Entities;

public class Season
{
    public int Year { get; set; }
    public string? Start { get; set; }
    public string? End { get; set; }
    public bool Current { get; set; }
    public Coverage? Fixtures { get; set; }
}
