using Microsoft.EntityFrameworkCore;
using Product.Service.Domain.Entity;
using Product.Service.Entity;

namespace Product.Service.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Products> Products => Set<Products>();
}