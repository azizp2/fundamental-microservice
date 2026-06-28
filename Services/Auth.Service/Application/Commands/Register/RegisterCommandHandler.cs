using Auth.Service.Application.Domain.Entities;
using Auth.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Exceptions;

namespace Auth.Service.Application.Commands.Register;

public class RegisterCommandHandler: IRequestHandler<RegisterCommand, RegisterCommandDto>
{
    private readonly AppDbContext _context;

    public RegisterCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterCommandDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);
        
        if (existingUser != null)
            throw new AppException(
                "user already exists.", 
                StatusCodes.Status400BadRequest);
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new Users()
        {
            Id = Guid.NewGuid(), 
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = "User",
            CreatedBy = "Admin",
            CreatedAt = DateTime.UtcNow
            
        };

        await _context.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterCommandDto(
            user.Id,
            user.Username,
            user.Email,
            user.Role);
    }
}