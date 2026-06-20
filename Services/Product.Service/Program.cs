using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Applications.Events.Consumers.Orders;
using Product.Service.Endpoints;
using Product.Service.Infrastructure.Data;
using Product.Service.Infrastructures.Messaging.Consumers;
using Shared.Common.Behaviors;
using Shared.Common.Middlewares;
using Shared.Contracts.Events.Orders;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Models;

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

#region RabbitMQ

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));

builder.Services.AddScoped<IEventConsumer<OrderCreatedEvent>, OrderCreatedConsumer>();
builder.Services.AddHostedService<OrderCreatedConsumerService>();

#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.MapProductEndpoints();
app.Run();
