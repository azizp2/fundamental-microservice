using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Product.Service.Infrastructures.Data;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Constants;

namespace Product.Service.Infrastructures.Outbox.Services;

public class OutboxProcessor
{
    private readonly AppDbContext _context;
    private readonly IEventPublisher _publisher;
    private readonly ILogger<OutboxProcessor> _logger;
    public OutboxProcessor(AppDbContext context, IEventPublisher publisher, ILogger<OutboxProcessor> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task ProcessAsync(
        CancellationToken cancellationToken)
    { 
        object? payload = null;
        string queueName = null;
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
                        
                        payload = JsonSerializer.Deserialize<OrderCreatedEvent>(message.Payload);
                        queueName = Queues.OrderCreated;

                        break;
                    
                    case nameof(ProductUnAvailableEvent):
                        if (string.IsNullOrEmpty(message.Payload))
                            continue;
                        
                        payload = JsonSerializer.Deserialize<ProductUnAvailableEvent>(message.Payload);
                        queueName = Queues.ProductUnAvailable;
                        
                        _logger.LogError($"Processing {queueName}");

                        break;
                }
                
                if(queueName == null)
                    throw new Exception($"Queue {queueName} not found");

                await _publisher.PublishAsync(
                    queueName,
                    payload,
                    cancellationToken
                );
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