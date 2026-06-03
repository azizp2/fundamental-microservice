using MediatR;
using Product.Service.Features.Shraed;

namespace Product.Service.Features.GetAllProduct;

public record GetProductsQuery(): IRequest<List<ProductDto>?>;