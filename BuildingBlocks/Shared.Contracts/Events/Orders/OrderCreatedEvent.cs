using Shared.Contracts.Abstractions;

namespace Shared.Contracts.Events.Orders;

public sealed class OrderCreatedEvent: IntegrationEvent
{
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }
    public List<OrerCreatedItem> Items { get; init; } = [];
}

public sealed class OrerCreatedItem
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
}
