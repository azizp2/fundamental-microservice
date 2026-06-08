using Order.Service.Application.Commands.CreateOrder;
using Order.Service.Commons.Constants;
using Shared.Common.Domain;

namespace Order.Service.Domain.Entity;

public class Orders : BaseEntity
{
    public Guid Id { get; set; } = Guid.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public Guid OrderUserId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal Total { get; set; }
    public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
}