using MediatR;
using Product.Service.Applications.Dtos;

namespace Product.Service.Applications.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id): IRequest<ProductDto?>;