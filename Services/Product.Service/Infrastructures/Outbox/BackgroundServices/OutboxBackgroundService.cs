using Product.Service.Infrastructures.Outbox.Services;

namespace Product.Service.Infrastructures.Outbox.BackgroundServices;

public sealed class OutboxBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope =
                scopeFactory.CreateScope();

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