using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Service.Endpoints;
using Order.Service.Infrastructure.Data;
using Order.Service.Infrastructure.Messaging.Publishers;
using Shared.Common.Behaviors;
using Shared.Common.Middlewares;
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


#region  RabbitMQ Configuration

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));
builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();

#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "Order Service Is Running....");

app.MapOrderEndpoints();

app.Run();
