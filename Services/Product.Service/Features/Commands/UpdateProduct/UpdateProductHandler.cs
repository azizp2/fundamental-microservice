using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Shared.Common.Exceptions;

namespace Product.Service.Features.Commands.UpdateProduct;

public class UpdateProductHandler(AppDbContext dbContext) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync(request.Id, cancellationToken);
        if(product is null) 
            throw new AppException("product not found.", StatusCodes.Status404NotFound);
        
        product.Name = request.ProductName;
        product.Price = request.Price;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }

}