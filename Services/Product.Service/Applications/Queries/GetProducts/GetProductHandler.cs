using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Applications.Dtos;
using Product.Service.Applications.Queries.GetProductById;
using Product.Service.Features.Queries.GetProductById;
using Product.Service.Infrastructure.Data;

namespace Product.Service.Applications.Queries.GetProducts;

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