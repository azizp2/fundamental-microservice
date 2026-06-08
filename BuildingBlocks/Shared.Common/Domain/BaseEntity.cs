namespace Shared.Common.Domain;

public abstract class BaseEntity
{
    public bool? IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ModifiedBy { get; set; } = string.Empty;
    public DateTime? ModifiedAt { get; set; }
}