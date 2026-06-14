namespace Auth.Service.Application.Commands.Register;

public sealed record RegisterCommandDto(Guid Id, string Username, string Email, string Role);
