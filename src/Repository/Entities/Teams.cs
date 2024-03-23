namespace PalpiteFC.Worker.Repository.Entities;

public class Teams : BaseEntity
{
    public string? Name { get; set; }
    public string? Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
