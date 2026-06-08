using Shared.Common.Domain;

namespace Product.Service.Domain.Entity;

public class Products: BaseEntity
{
    public Guid  Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock  { get; set; }
}