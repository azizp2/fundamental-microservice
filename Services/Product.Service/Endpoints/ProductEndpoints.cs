using MediatR;
using Microsoft.AspNetCore.Mvc;
using Product.Service.Applications.Commands.UpdateProduct;
using Product.Service.Applications.Dtos;
using Product.Service.Applications.Queries.GetProductById;
using Product.Service.Features.Queries.GetProductById;
using Shared.Common.Exceptions;
using Shared.Common.Responses;

namespace Product.Service.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products");

        group.MapGet("/getAll", GetAll);
        group.MapGet("/getById/{id:guid}", GetById).RequireAuthorization();
        group.MapPost("/create", Create);
        group.MapPut("/update/{id:guid}", Update);
        group.MapDelete("/delete/{id:guid}", Delete);

        return endpoints;
    }

    private static async Task<IResult> GetAll([FromServices] IMediator  mediator, [AsParameters] GetProductsQuery query)
    {
        var products = await mediator.Send(query);
        if (products is null)
            throw new AppException("products not found.", StatusCodes.Status404NotFound);
        
        var results = ApiResponse<List<ProductDto>>.Ok(products!);

        return Results.Ok(results);
    }

    private static async Task<IResult> GetById([FromServices] IMediator mediator, [AsParameters] GetProductByIdQuery query)
    {
        var products = await mediator.Send(query);
        if (products is null)
            throw new AppException("products not found.", StatusCodes.Status404NotFound);
        
        var results = ApiResponse<ProductDto>.Ok(products!);

        return Results.Ok(results);
    }

    private static IResult Create()
    {
        return Results.Created();
    }

    private static async Task<IResult> Update(Guid id, UpdateProductDto param, [FromServices] IMediator mediator)
    {
        var command = new UpdateProductCommand(
            id, 
            param.ProductName, 
            param.Price
        );
        
        await mediator.Send(command);
        
        var result = ApiResponse<string>.Ok(null!);
        
        return Results.Ok(result);
    }

    private static IResult Delete(Guid id)
    {
        return Results.NoContent();
    }
}