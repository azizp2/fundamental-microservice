using MediatR;
using Product.Service.Features.Shraed;

namespace Product.Service.Features.UpdateProduct;

public record UpdateProductCommand(UpdateProductDto param, Guid Id): IRequest<Unit>;