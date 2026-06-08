using Order.Service.Commons.Constants;

namespace Order.Service.Application.Commands.CreateOrder;

public class CreateOrderDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } =  string.Empty; 
    public string OrderUserName { get; set; } = string.Empty;
    public DateTime? OrderDate { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemsDto> OrderItems { get; set; } = new();
}

public class OrderItemsDto
{
    public Guid Id { get; set; } 
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal? Discount { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
}