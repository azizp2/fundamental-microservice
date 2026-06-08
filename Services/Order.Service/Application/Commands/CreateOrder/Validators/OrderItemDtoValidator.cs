using FluentValidation;

namespace Order.Service.Application.Commands.CreateOrder.Validators;

public sealed class OrderItemDtoValidator
    : AbstractValidator<OrderItemsDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(200);
        
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Qty)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}