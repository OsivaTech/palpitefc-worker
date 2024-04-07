namespace PalpiteFC.Worker.Repository.Entities;
public class UserPoints : BaseEntity
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int Points { get; set; }
    public int PointSeasonId { get; set; }
}
