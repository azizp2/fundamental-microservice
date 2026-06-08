namespace Shared.Contracts.Events.Orders;

public sealed class OrerCreatedItem
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public int Qty { get; init; }
}
