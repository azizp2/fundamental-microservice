using MediatR;
using Product.Service.Features.Dtos;

namespace Product.Service.Features.Queries.GetProductById;

public record GetProductsQuery(): IRequest<List<ProductDto>?>;