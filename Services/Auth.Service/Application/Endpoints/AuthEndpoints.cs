using Auth.Service.Application.Commands.Login;
using Auth.Service.Infrastructure.Authentications.JwtServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Responses;

namespace Auth.Service.Application.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");
 
        // group.MapGet("/getAll", GetAll);
        // group.MapGet("/getById/{id:guid}", GetById);
        group.MapPost("/register", Register);
        // group.MapPut("/update/{id:guid}", Update);
        // group.MapDelete("/delete/{id:guid}", Delete);
 
        return endpoints;
    }

    private static async Task<IResult> Register([FromServices] IMediator mediator, [FromBody] LoginCommand command)
    {
        var user = await mediator.Send(command);
        
        var result = ApiResponse<LoginCommandDto>.Ok(user, "create user successfully.");
        
        return Results.Ok(result);
    }

    // private static async Task<IResult> Create([FromServices] IMediator mediator, [FromBody] CreateOrderCommand command)
    // {
    //     var order  = await mediator.Send(command);
    //
    //     var result = ApiResponse<CreateOrderDto>.Ok(order, "create order successfully.");
    //      
    //     return Results.Ok(result);
    // }
}