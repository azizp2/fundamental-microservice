using System.Security.Claims;
using System.Text;
using Auth.Service.Application.Domain.Entities;
using Auth.Service.Infrastructure.Authentications.JwtServices;
using Auth.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Common.Exceptions;

namespace Auth.Service.Application.Commands.Login;

public class LoginCommandHandler: IRequestHandler<LoginCommand, LoginCommandDto>
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenGenerator _jwtService;

    public LoginCommandHandler(AppDbContext dbContext, IJwtTokenGenerator jwtService)
    {
        _context = dbContext;
        _jwtService = jwtService;
    }

    public async Task<LoginCommandDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username.ToLower() == request.UserName.ToLower()
                                      || x.Email.ToLower() == request.UserName.ToLower(), cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password.");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!isValidPassword)
            throw new AppException("Invalid username or password.", StatusCodes.Status401Unauthorized);

        var token = _jwtService.GenerateToken(user);

        return new LoginCommandDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = token
        };
    }
}