using Product.Service.Entity;

namespace Product.Service.Applications.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    int Stock
);