using UserManagement.Application.Users;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Abstractions;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(UserDto user);
}
