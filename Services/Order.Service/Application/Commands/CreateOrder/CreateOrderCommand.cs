using MediatR;

namespace Order.Service.Application.Commands.CreateOrder;

public class CreateOrderCommand() : IRequest<CreateOrderDto?>
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemsDto> OrderItems { get; set; } = new();
}
