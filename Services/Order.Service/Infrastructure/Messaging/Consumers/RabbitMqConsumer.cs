using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Models;

namespace Order.Service.Infrastructure.Messaging.Consumers;

public class RabbitMqConsumer<T> : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly IEventConsumer<T> _consumer;
    private readonly string _queueName;

    public RabbitMqConsumer(
        IOptions<RabbitMqSettings> options,
        IEventConsumer<T> consumer,
        string queueName)
    {
        _settings = options.Value;
        _consumer = consumer;
        _queueName = queueName;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                var message = JsonSerializer.Deserialize<T>(json);

                if (message is not null)
                {
                    await _consumer.ConsumeAsync(
                        message,
                        stoppingToken);
                }

                await channel.BasicAckAsync(
                    ea.DeliveryTag,
                    false,
                    stoppingToken);
            }
            catch
            {
                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    false,
                    true,
                    stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}