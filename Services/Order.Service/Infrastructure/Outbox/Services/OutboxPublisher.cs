using System.Text.Json;
using Order.Service.Infrastructure.Outbox.Entity;
using Shared.Contracts.Abstractions;

namespace Order.Service.Infrastructure.Outbox.Services;

public sealed class OutboxPublisher
{
    public OutboxMessage Create<T>(T integrationEvent) where  T : IntegrationEvent
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventId = integrationEvent.EventId,
            EventType = typeof(T).Name,
            Payload = JsonSerializer.Serialize(integrationEvent),
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
        };
    }
    
}