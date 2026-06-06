using MediatR;
using Product.Service.Features.Dtos;

namespace Product.Service.Features.Commands.UpdateProduct;

public record UpdateProductCommand(UpdateProductDto param, Guid Id): IRequest<Unit>;