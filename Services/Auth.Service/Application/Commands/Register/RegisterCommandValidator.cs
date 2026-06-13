using FluentValidation;

namespace Auth.Service.Application.Commands.Register;

public class RegisterCommandValidator: AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("username is required");
        
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty().WithMessage("email is required");
       
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("password confirmation is required");
        
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("passwords do not match");
    }
}