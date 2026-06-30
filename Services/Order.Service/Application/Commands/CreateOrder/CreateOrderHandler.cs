using MediatR;
using Order.Service.Commons.Constants;
using Order.Service.Domain.Entity;
using Order.Service.Infrastructure.Data;
using Order.Service.Infrastructure.Outbox.Services;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Constants;

namespace Order.Service.Application.Commands.CreateOrder;

public class CreateOrderHandler(AppDbContext context, OutboxPublisher outboxPublisher) : IRequestHandler<CreateOrderCommand, CreateOrderDto>
{
    public async Task<CreateOrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Orders
        {
            Id = Guid.NewGuid(),
            OrderDate = request.OrderDate,
            OrderUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Admins"
        };
        order.Total = request.OrderItems.Sum(x => x.Qty * x.Price);
        order.OrderItems = request.OrderItems.Select(x => new OrderItems
        {
            Id = Guid.NewGuid(),
            OrderId =  order.Id,
            ProductId = x.ProductId,
            ProductName = x.ProductName,
            Qty = x.Qty,
            Discount = x.Discount,
            Price = x.Price,
            CreatedBy = "Admins",
            CreatedAt = DateTime.UtcNow
        }).ToList();

        context.Orders.Add(order);

        var createdOrderEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            TotalAmount = order.Total,
            Items = order.OrderItems.Select(x => new OrerCreatedItem
            {
                OrderId =  order.Id,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Qty = x.Qty
            }).ToList()
        };

        var outboxMessage = outboxPublisher.Create(createdOrderEvent);
        context.OutboxMessages.Add(outboxMessage);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return new CreateOrderDto
        {
           Id =  order.Id,
        };
    }
}

