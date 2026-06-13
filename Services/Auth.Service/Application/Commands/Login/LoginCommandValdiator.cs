using FluentValidation;

namespace Auth.Service.Application.Commands.Login;

public class LoginCommandValdiator:AbstractValidator<LoginCommand>
{
    public LoginCommandValdiator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required")
            .NotNull().WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}