namespace Product.Service.Entity;

public abstract class BaseEntity
{
    public bool? IsActive { get; set; } = true;
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string ModifiedBy { get; init; } = string.Empty;
    public DateTime ModifiedAt { get; init; }
}