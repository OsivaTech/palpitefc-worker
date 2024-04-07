namespace PalpiteFC.Worker.Repository.Entities;

public class Fixtures : BaseEntity
{
    public string? Name { get; set; }
    public int ChampionshipId { get; set; }
    public DateTime Start { get; set; }
    public bool Finished { get; set; }
}
