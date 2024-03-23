namespace PalpiteFC.Worker.Integrations.Entities;

public class ApiFootballResult<T>
{
    public string? Get { get; set; }
    public Parameters? Parameters { get; set; }
    public object[]? Errors { get; set; }
    public int Results { get; set; }
    public Paging? Paging { get; set; }
    public IEnumerable<T>? Response { get; set; }
}

public class Parameters
{
    public string? League { get; set; }
    public string? Season { get; set; }
}

public class Paging
{
    public int Current { get; set; }
    public int Total { get; set; }
}