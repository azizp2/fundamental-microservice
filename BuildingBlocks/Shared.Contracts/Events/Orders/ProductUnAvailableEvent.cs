using Shared.Contracts.Abstractions;

namespace Shared.Contracts.Events.Orders;

public class ProductUnAvailableEvent: IntegrationEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}