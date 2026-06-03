using Product.Service.Entity;

namespace Product.Service.Features.Shraed;

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    int Stock
);