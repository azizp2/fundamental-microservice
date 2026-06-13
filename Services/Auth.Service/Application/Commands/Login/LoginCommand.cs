using MediatR;

namespace Auth.Service.Application.Commands.Login;

public class LoginCommand: IRequest<LoginCommandDto>
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}