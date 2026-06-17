using MediatR;
using Product.Service.Applications.Dtos;

namespace Product.Service.Applications.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, string ProductName, decimal Price): IRequest<Unit>;