namespace Product.Service.Entity;

public class Products: BaseEntity
{
    public Guid  Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock  { get; init; }
}