using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructure.Outbox.Entity;
using Product.Service.Infrastructure.Outbox.Services;
using Product.Service.Infrastructures.Data;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;

namespace Product.Service.Applications.Events.Consumers.Orders;

public class OrderCreatedConsumer
    : IEventConsumer<OrderCreatedEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly OutboxPublisher _outboxPublisher;

    public OrderCreatedConsumer(AppDbContext context, ILogger<OrderCreatedConsumer> logger, OutboxPublisher outboxPublisher)
    {
        _context = context;
        _logger = logger;
        _outboxPublisher = outboxPublisher;
    }

    public async Task ConsumeAsync(
        OrderCreatedEvent message,
        CancellationToken cancellationToken = default)
    {
        try
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
                    
                    await PublishUnAvailableEvent(item, cancellationToken);
                    
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task PublishUnAvailableEvent(OrerCreatedItem item, CancellationToken cancellationToken)
    {
        var @event = new ProductUnAvailableEvent
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Reason = $"Product {item.ProductName} is unavailable",
        };

        var outboxMessage = _outboxPublisher.Create(@event);
        _context.OutboxMessages.Add(outboxMessage);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}