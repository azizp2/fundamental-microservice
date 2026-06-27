using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger JSON milik Gateway (tidak masalah tetap ada)
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";

    // JSON dari Auth Service
    c.SwaggerEndpoint("/swagger/auth/v1/swagger.json", "Auth API");

    // JSON dari Product Service
    c.SwaggerEndpoint("/swagger/product/v1/swagger.json", "Product API");

    // JSON dari Order Service
    c.SwaggerEndpoint("/swagger/order/v1/swagger.json", "Order API");
});

// Reverse proxy
app.MapReverseProxy();

app.Run();