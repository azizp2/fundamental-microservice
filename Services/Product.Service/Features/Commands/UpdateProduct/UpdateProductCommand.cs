using MediatR;
using Product.Service.Features.Dtos;

namespace Product.Service.Features.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, string ProductName, decimal Price): IRequest<Unit>;