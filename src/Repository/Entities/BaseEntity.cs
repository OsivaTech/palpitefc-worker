namespace PalpiteFC.Worker.Repository.Entities;

public abstract class BaseEntity
{
    public virtual int Id { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime UpdatedAt { get; set; }
}
