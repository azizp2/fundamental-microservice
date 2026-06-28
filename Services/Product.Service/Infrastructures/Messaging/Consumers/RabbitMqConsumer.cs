using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Product.Service.Commons.Enums;
using Product.Service.Infrastructures.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Constants;
using Shared.RabbitMQ.Models;

namespace Product.Service.Infrastructures.Messaging.Consumers;

public abstract class RabbitMqConsumer<T> : BackgroundService
{
    protected abstract string QueueName { get; }
    protected virtual string DeadLetterQueue => $"{QueueName}.dlq";

    private  readonly RabbitMqSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    
    private IConnection? _connection;
    private IChannel? _channel;

    protected RabbitMqConsumer(
        IServiceProvider serviceProvider,
        IOptions<RabbitMqSettings> options, 
        ILogger<RabbitMqConsumer<T>> logger)
    {
        _settings = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };
        
        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);
        
        
        await _channel.ExchangeDeclareAsync(
            exchange: Exchanges.DeadLetter,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
        
        //--------------------------------------------------
        // Dead Letter Queue
        //--------------------------------------------------
        
        await _channel.QueueDeclareAsync(
            queue: DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
        
        await _channel.QueueBindAsync(
            queue: DeadLetterQueue,
            exchange: Exchanges.DeadLetter,
            routingKey: QueueName,
            cancellationToken: cancellationToken);
        
        
        //--------------------------------------------------
        // Main Queue
        //--------------------------------------------------

        var arguments = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = Exchanges.DeadLetter,
            ["x-dead-letter-routing-key"] = QueueName
        };
        
        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken);
        
        //--------------------------------------------------
        // Prefetch
        //--------------------------------------------------
        
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: cancellationToken);
        
        //--------------------------------------------------
        // Consumer
        //--------------------------------------------------
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            Domain.Entity.EventLog? eventLog = null;
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogInformation(
                    "Received message from {Queue}",
                    QueueName);

                var message = JsonSerializer.Deserialize<T>(json);

                if (message is null)
                {
                    throw new InvalidOperationException(
                        $"Unable to deserialize message for queue {QueueName}");
                }

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                eventLog = new Domain.Entity.EventLog
                {
                    Id = Guid.NewGuid(),
                    EventType = typeof(T).Name,
                    QueueName = QueueName,
                    Payload = json,
                    Status = (int)EventStatus.Processing,
                    RetryCount = 0,
                    CreatedAt = DateTime.UtcNow
                };
                
                db.EventLogs.Add(eventLog);
                await db.SaveChangesAsync(cancellationToken);

                var eventConsumer =
                    scope.ServiceProvider
                        .GetRequiredService<IEventConsumer<T>>();

                await eventConsumer.ConsumeAsync(
                    message,
                    cancellationToken);
                
                eventLog.Status = (int)EventStatus.Success;;
                eventLog.ProcessedAt = DateTime.UtcNow;

                await db.SaveChangesAsync(cancellationToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Message acknowledged. Queue: {Queue}",
                    QueueName);
            }
            catch (Exception ex)
            {
                if (eventLog != null)
                {
                    using var scope = _serviceProvider.CreateScope();

                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var log = await db.EventLogs.FindAsync(eventLog.Id);

                    if (log != null)
                    {
                        log.Status = (int)EventStatus.Failed;;
                        log.ErrorMessage = ex.ToString();
                        log.ProcessedAt = DateTime.UtcNow;

                        await db.SaveChangesAsync(cancellationToken);
                    }
                }
                _logger.LogError(
                    ex,
                    "Message processing failed. Queue: {Queue}. Sending to DLQ.",
                    QueueName);

                await _channel.BasicRejectAsync(
                    deliveryTag: ea.DeliveryTag,
                    requeue: false,
                    cancellationToken: cancellationToken);
            }
        };
        
        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "RabbitMQ Consumer started. Queue: {Queue}",
            QueueName);

        await Task.Delay(
            Timeout.Infinite,
            cancellationToken);
    }
    
    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}