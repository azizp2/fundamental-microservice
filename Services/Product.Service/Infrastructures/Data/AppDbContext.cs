using Microsoft.EntityFrameworkCore;
using Product.Service.Domain.Entity;
using Product.Service.Infrastructure.Outbox.Entity;

namespace Product.Service.Infrastructures.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Products> Products => Set<Products>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<EventLog> EventLogs => Set<EventLog>();
}