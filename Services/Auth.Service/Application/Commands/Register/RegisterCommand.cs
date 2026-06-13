using MediatR;

namespace Auth.Service.Application.Commands.Register;

public sealed record RegisterCommand(
    string Username,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<RegisterCommandDto>;
