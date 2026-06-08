using FluentValidation;

namespace Order.Service.Application.Commands.CreateOrder.Validators;

public class CreateOrderValidator: AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required.");

        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .WithMessage("Order must contain at least one item.");
        
        RuleFor(x => x.OrderDate)
            .NotEmpty().WithMessage("OrderDate is required.");
        
        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemDtoValidator());
    }
}