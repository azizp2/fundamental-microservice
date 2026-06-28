using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructures.Data;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Constants;

namespace Product.Service.Infrastructure.Outbox.Services;

public class OutboxProcessor
{
    private readonly AppDbContext _context;
    private readonly IEventPublisher _publisher;

    public OutboxProcessor(AppDbContext context, IEventPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task ProcessAsync(
        CancellationToken cancellationToken)
    {
        var messages = await _context.OutboxMessages
            .Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.CreatedAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                switch (message.EventType)
                {
                    case nameof(OrderCreatedEvent):
                        if (string.IsNullOrEmpty(message.Payload))
                            continue;
                        
                        var payload =  JsonSerializer.Deserialize<OrderCreatedEvent>(message.Payload);

                        await _publisher.PublishAsync(
                            Queues.OrderCreated,
                            payload,
                            cancellationToken
                            );
                        break;
                }

                 message.ProcessedAt = DateTime.UtcNow;
            }
            catch(Exception ex)
            {
                message.ErrorMessage =  ex.Message; 
                message.RetryCount++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}