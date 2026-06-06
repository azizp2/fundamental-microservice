using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Service.Data;
using Product.Service.Features.GetAllProduct;
using Product.Service.Features.GetProductById;
using Product.Service.Features.Shraed;
using Product.Service.Features.UpdateProduct;
using Shared.Common.Exceptions;

namespace Product.Service.Features.UpdateProduct;

public class UpdateProductHandler(AppDbContext dbContext) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync(request.Id, cancellationToken);
        if(product is null) 
            throw new AppException("product not found.", StatusCodes.Status404NotFound);
        
        product.Name = request.param.Name;
        product.Price = request.param.Price;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }

}