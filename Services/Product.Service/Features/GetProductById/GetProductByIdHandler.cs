using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Product.Service.Features.GetAllProduct;
using Product.Service.Features.GetProductById;
using Product.Service.Features.Shraed;

namespace Product.Service.Features.GetProductById;

public class GetProductByIdHandler(AppDbContext dbContext) : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new ProductDto(
                x.Id,
                x.Name,
                x.Price,
                x.Stock))
            .FirstOrDefaultAsync(cancellationToken);
    }
    
}