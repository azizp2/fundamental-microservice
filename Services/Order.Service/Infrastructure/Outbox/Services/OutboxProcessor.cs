using Microsoft.EntityFrameworkCore;
using Order.Service.Infrastructure.Data;

namespace Order.Service.Infrastructure.Outbox.Services;

public class OutboxProcessor
{
    private readonly AppDbContext _context;

    public OutboxProcessor(AppDbContext context)
    {
        _context = context;
    }

    public async Task ProcessAsync(
        CancellationToken cancellationToken)
    {
        var messages = await _context.OutboxMessages
            .Where(x => x.ProcessedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                // TODO:
                // Publish RabbitMQ

                message.ProcessedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                message.RetryCount++;

                message.ErrorMessage = ex.Message;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}