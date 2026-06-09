using Order.Service.Infrastructure.Outbox.Services;

namespace Order.Service.Infrastructure.Outbox.BackgroundServices;

public sealed class OutboxBackgroundService
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxBackgroundService(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope =
                _scopeFactory.CreateScope();

            var processor =
                scope.ServiceProvider
                    .GetRequiredService<OutboxProcessor>();

            await processor.ProcessAsync(
                stoppingToken);

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }
}