namespace ScholaPlan.Domain.Contract;

public class EntityBase
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = UpdatedAt = DateTime.Now;
    }

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}