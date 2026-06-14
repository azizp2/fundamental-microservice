using MediatR;
 using Microsoft.AspNetCore.Mvc;
 using Order.Service.Application.Commands.CreateOrder;
 using Shared.Common.Responses;
 
 namespace Order.Service.Endpoints;
 
 public static class OrderEndpoints
 {
     public static IEndpointRouteBuilder MapOrderEndpoints(
         this IEndpointRouteBuilder endpoints)
     {
         var group = endpoints.MapGroup("/api/orders");
 
         // group.MapGet("/getAll", GetAll);
         // group.MapGet("/getById/{id:guid}", GetById);
         group.MapPost("/create", Create).RequireAuthorization();
         // group.MapPut("/update/{id:guid}", Update);
         // group.MapDelete("/delete/{id:guid}", Delete);
 
         return endpoints;
     }
 
     private static async Task<IResult> Create([FromServices] IMediator mediator, [FromBody] CreateOrderCommand command)
     {
         var order  = await mediator.Send(command);
 
         var result = ApiResponse<CreateOrderDto>.Ok(order, "create order successfully.");
         
         return Results.Ok(result);
     }
 }