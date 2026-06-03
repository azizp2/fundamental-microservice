using MediatR;
using Microsoft.AspNetCore.Mvc;
using Product.Service.Data;
using Product.Service.Features.GetAllProduct;

namespace Product.Service.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products");

        group.MapGet("/getAll", GetAll);
        group.MapGet("/getById{id:guid}", GetById);
        group.MapPost("/create", Create);
        group.MapPut("/update/{id:guid}", Update);
        group.MapDelete("/delete/{id:guid}", Delete);

        return endpoints;
    }

    private static async Task<IResult> GetAll([FromServices] IMediator  mediator, [AsParameters] GetProductsQuery query)
    {
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    private static IResult GetById(Guid id)
    {
        return Results.Ok(id);
    }

    private static IResult Create()
    {
        return Results.Created();
    }

    private static IResult Update(Guid id)
    {
        return Results.Ok(id);
    }

    private static IResult Delete(Guid id)
    {
        return Results.NoContent();
    }
}