using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Models;

namespace Product.Service.Infrastructures.Messaging.Consumers;

public abstract class RabbitMqConsumer<T> : BackgroundService
{
    protected abstract string QueueName { get; }

    private  readonly RabbitMqSettings _settings;
    private readonly IServiceProvider _serviceProvider;

    protected RabbitMqConsumer(
        IServiceProvider serviceProvider,
        IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
        _serviceProvider = serviceProvider;
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
            queue: QueueName,
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
                
                using var scope = _serviceProvider.CreateScope();
                
                var eventConsumer =
                    scope.ServiceProvider
                        .GetRequiredService<IEventConsumer<T>>();

                if (message is not null)
                {
                    await eventConsumer.ConsumeAsync(
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
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}