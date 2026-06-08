using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Models;

namespace Order.Service.Infrastructure.Messaging.Publishers;

public class RabbitMqPublisher : IEventPublisher
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqPublisher(
        IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
    }

    public async Task PublishAsync<T>(
        string queueName,
        T message,
        CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);

        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body,
            cancellationToken: cancellationToken);
    }
}