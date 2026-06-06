using MediatR;
using Product.Service.Features.Shraed;

namespace Product.Service.Features.UpdateProduct;

public record UpdateProductQuery(UpdateProductDto param, Guid Id): IRequest<Unit>;