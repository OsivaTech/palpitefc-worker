namespace PalpiteFC.Worker.Repository.Entities;
public class UserPoints : BaseEntity
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int Points { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
