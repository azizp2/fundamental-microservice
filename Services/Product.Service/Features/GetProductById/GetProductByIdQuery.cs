using MediatR;
using Product.Service.Features.Shraed;

namespace Product.Service.Features.GetProductById;

public record GetProductByIdQuery(Guid Id): IRequest<ProductDto?>;