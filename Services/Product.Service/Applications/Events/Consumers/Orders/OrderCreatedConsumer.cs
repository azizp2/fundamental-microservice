using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructures.Data;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;

namespace Product.Service.Applications.Events.Consumers.Orders;

public class OrderCreatedConsumer
    : IEventConsumer<OrderCreatedEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(AppDbContext context, ILogger<OrderCreatedConsumer> logger)
    {
        _context = context;
        _logger = logger;
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
                _logger.LogError(
                    "Product not found. ProductId: {ProductId}",
                    item.ProductId);
                
                throw new InvalidOperationException(
                    $"Product {item.ProductId} not found");
            }

            if (product.Stock < item.Qty)
            {
                _logger.LogWarning(
                    "Insufficient stock. ProductId: {ProductId}, Stock: {Stock}, Requested: {Qty}",
                    product.Id,
                    product.Stock,
                    item.Qty);
                
                throw new InvalidOperationException(
                    $"Insufficient stock for product {product.Id}");
            }

            product.Stock -= item.Qty;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}