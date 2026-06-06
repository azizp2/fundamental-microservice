using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Product.Service.Features.Dtos;
using Product.Service.Features.Queries.GetProductById;

namespace Product.Service.Features.Queries.GetProducts;

public class GetProductHandler(AppDbContext dbContext) : IRequestHandler<GetProductsQuery, List<ProductDto>?>
{
    public async Task<List<ProductDto>?> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Select(x => new ProductDto(
                x.Id,
                x.Name,
                x.Price,
                x.Stock))
            .ToListAsync(cancellationToken);
    }
    
}