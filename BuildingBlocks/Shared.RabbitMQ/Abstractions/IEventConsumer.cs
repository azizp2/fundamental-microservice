namespace Shared.RabbitMQ.Abstractions;

public interface IEventConsumer<in T>
{
    Task ConsumeAsync(
        T message,
        CancellationToken cancellationToken = default);
}