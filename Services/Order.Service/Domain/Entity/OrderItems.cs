using Shared.Common.Domain;

namespace Order.Service.Domain.Entity;

public class OrderItems: BaseEntity
{
    public Guid Id { get; set; } 
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public int Qty { get; set; }
}