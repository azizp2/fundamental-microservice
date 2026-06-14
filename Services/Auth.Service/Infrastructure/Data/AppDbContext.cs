using Auth.Service.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Users>  Users => Set<Users>();
}