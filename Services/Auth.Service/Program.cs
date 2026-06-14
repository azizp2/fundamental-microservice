using Auth.Service.Endpoints;
using Auth.Service.Infrastructure.Authentications;
using Auth.Service.Infrastructure.Authentications.JwtServices;
using Auth.Service.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Behaviors;
using Shared.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

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

#region MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
#endregion

#region DependencyInjection

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "Auth Service Running...");

app.MapAuthEndpoints();

app.Run();
