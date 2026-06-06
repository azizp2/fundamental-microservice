using MediatR;
using Product.Service.Features.Dtos;

namespace Product.Service.Features.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id): IRequest<ProductDto?>;