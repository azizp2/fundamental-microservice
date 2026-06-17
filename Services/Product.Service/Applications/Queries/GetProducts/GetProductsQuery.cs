using MediatR;
using Product.Service.Applications.Dtos;

namespace Product.Service.Features.Queries.GetProductById;

public record GetProductsQuery(): IRequest<List<ProductDto>?>;