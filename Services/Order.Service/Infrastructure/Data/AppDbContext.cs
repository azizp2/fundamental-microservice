using Microsoft.EntityFrameworkCore;
using Order.Service.Domain.Entity;

namespace Order.Service.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Orders>  Orders => Set<Orders>();
    public DbSet<OrderItems>  OrderItems =>  Set<OrderItems>();
}