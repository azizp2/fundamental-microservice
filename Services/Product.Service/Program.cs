using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Product.Service.Endpoints;
using Shared.Common.Behaviors;
using Shared.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention();;
});

#region FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);
#endregion

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.MapProductEndpoints();
app.Run();
