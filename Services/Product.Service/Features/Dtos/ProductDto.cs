using Product.Service.Entity;

namespace Product.Service.Features.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    int Stock
);

public record UpdateProductDto(
    string Name,
    decimal Price
);