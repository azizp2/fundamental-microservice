using FluentValidation;

namespace Product.Service.Features.Commands.UpdateProduct;

public class UpdateProductValidator: AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MinimumLength(5)
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");
    }
}