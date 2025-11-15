using System.Security.Cryptography;
using System.Text;
using UserManagement.Application.Abstractions;
using UserManagement.Application.Users;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repo;
    private readonly IJwtTokenService _jwt;

    public AuthService(IUserRepository repo, IJwtTokenService jwt)
    {
        _repo = repo;
        _jwt = jwt;
    }

    public async Task<(string Token, DateTime ExpiresAt)?> LoginAsync(
     string username,
     string password,
     CancellationToken ct = default)
    {
        var user = await _repo.GetByUsernameAsync(username, ct);
        if (user is null)
            return null;

        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        // Map domain User -> UserDto for JWT
        var dto = new UserDto(
            user.Id,
            user.Username,
            user.Email,
            (UserRole)user.Role,
            user.CreatedAt,
            user.UpdatedAt
        );

        var (token, expiresAt) = _jwt.GenerateToken(dto);
        return (token, expiresAt);
    }


    private static bool VerifyPassword(string password, string storedHash)
    {
        var data = Convert.FromBase64String(storedHash);
        var salt = data[..16];
        var key = data[16..];

        using var derive = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var computed = derive.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(computed, key);
    }
}
