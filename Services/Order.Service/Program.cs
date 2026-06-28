using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order.Service.Endpoints;
using Order.Service.Infrastructure.Authentications;
using Order.Service.Infrastructure.Data;
using Order.Service.Infrastructure.Messaging.Publishers;
using Order.Service.Infrastructure.Outbox.BackgroundServices;
using Order.Service.Infrastructure.Outbox.Services;
using Shared.Common.Behaviors;
using Shared.Common.Middlewares;
using Shared.RabbitMQ.Abstractions;
using Shared.RabbitMQ.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention();;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });

builder.Services.AddAuthorization();


#region FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);
#endregion

#region MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
#endregion

#region  RabbitMQ Configuration

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));
builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();

#endregion

#region  Outbox Configuration
builder.Services.AddScoped<OutboxProcessor>();
builder.Services.AddSingleton<OutboxPublisher>();
builder.Services.AddHostedService<OutboxBackgroundService>();
#endregion

var app = builder.Build();

app.UseSwagger();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Order Service Is Running....");

app.MapOrderEndpoints();

app.Run();
