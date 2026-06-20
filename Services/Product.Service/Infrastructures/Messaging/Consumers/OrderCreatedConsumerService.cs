using Microsoft.Extensions.Options;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Constants;
using Shared.RabbitMQ.Models;

namespace Product.Service.Infrastructures.Messaging.Consumers;

public class OrderCreatedConsumerService
    : RabbitMqConsumer<OrderCreatedEvent>
{
    protected override string QueueName => Queues.OrderCreated;

    public OrderCreatedConsumerService(
        IServiceProvider serviceProvider,
        IOptions<RabbitMqSettings> options)
        : base(serviceProvider, options) { }
}