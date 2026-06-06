using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Product.Service.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention();;
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

app.MapProductEndpoints();
app.Run();
