namespace PalpiteFC.Worker.Repository.Entities;

public class Championships : BaseEntity
{
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
