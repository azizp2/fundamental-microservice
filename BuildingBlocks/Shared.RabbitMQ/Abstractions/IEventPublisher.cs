namespace Shared.RabbitMQ.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<T>(
        string queueName,
        T message,
        CancellationToken cancellationToken = default);
}