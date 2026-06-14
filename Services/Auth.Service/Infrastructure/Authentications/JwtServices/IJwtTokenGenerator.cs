using Auth.Service.Application.Domain.Entities;

namespace Auth.Service.Infrastructure.Authentications.JwtServices;

public interface IJwtTokenGenerator
{
    public string GenerateToken(Users user);
}