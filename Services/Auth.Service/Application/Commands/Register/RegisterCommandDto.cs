namespace Auth.Service.Application.Commands.Register;

public class RegisterCommandDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role  { get; set; } = string.Empty;
}