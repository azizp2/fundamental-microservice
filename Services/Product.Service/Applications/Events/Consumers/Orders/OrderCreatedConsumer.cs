using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructure.Data;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;

namespace Product.Service.Applications.Events.Consumers.Orders;

public class OrderCreatedConsumer
    : IEventConsumer<OrderCreatedEvent>
{
    private readonly AppDbContext _context;

    public OrderCreatedConsumer(AppDbContext context)
    {
        _context = context;
    }

    public async Task ConsumeAsync(
        OrderCreatedEvent message,
        CancellationToken cancellationToken = default)
    {
        foreach (var item in message.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(
                    x => x.Id == item.ProductId,
                    cancellationToken);

            if (product is null)
            {
                throw new InvalidOperationException(
                    $"Product {item.ProductId} not found");
            }

            if (product.Stock < item.Qty)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product {product.Id}");
            }

            product.Stock -= item.Qty;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}