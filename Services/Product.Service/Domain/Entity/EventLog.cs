namespace Product.Service.Domain.Entity;

public class EventLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string EventType { get; set; } = string.Empty;

    public string? QueueName { get; set; }

    public string Payload { get; set; } = string.Empty;

    public int Status { get; set; }

    public int RetryCount { get; set; } = 0;

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }
}