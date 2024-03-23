namespace PalpiteFC.Worker.Repository.Entities;

public class TeamsGame : BaseEntity
{
    public int Gol { get; set; }
    public int TeamId { get; set; }
    public int GameId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
