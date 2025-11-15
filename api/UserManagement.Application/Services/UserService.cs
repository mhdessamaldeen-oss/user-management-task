using System.Security.Cryptography;
using UserManagement.Application.Abstractions;
using UserManagement.Application.Models;
using UserManagement.Application.Users;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IAuditService _audit;

    public UserService(IUserRepository repo, IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<UserDto> CreateAsync(
        CreateUserRequest request,
        string performedBy,
        string? ip,
        CancellationToken ct = default)
    {
        if (await _repo.UsernameExistsAsync(request.Username, null, ct))
            throw new InvalidOperationException("Username already exists.");

        var now = DateTime.UtcNow;

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = HashPassword(request.Password),
            Role = (int)request.Role,
            CreatedAt = now,
            CreatedBy = performedBy,
            UpdatedAt = now,
            LastModifiedBy = performedBy
        };

        await _repo.AddAsync(user, ct);

        await _audit.LogAsync(
            AuditAction.Insert.ToString(),
            "User",
            $"Id={user.Id}",
            performedBy,
            ip,
            null,
            ct);

        return Map(user);
    }

    public async Task<UserDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct);
        return user is null ? null : Map(user);
    }

    public async Task<UserDto?> UpdateAsync(
        int id,
        UpdateUserRequest request,
        string performedBy,
        string? ip,
        CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct);
        if (user is null) return null;

        user.Email = request.Email.Trim();
        user.Role = (int)request.Role;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
            user.PasswordHash = HashPassword(request.NewPassword);

        user.UpdatedAt = DateTime.UtcNow;
        user.LastModifiedBy = performedBy;

        await _repo.UpdateAsync(user, ct);

        await _audit.LogAsync(
            AuditAction.Update.ToString(),
            "User",
            $"Id={user.Id}",
            performedBy,
            ip,
            null,
            ct);

        return Map(user);
    }

    public async Task<UserDto?> UpdateProfileAsync(
        int userId,
        UpdateUserProfileRequest request,
        string performedBy,
        string? ip,
        CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(userId, ct);
        if (user is null) return null;

        user.Email = request.Email.Trim();

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
            user.PasswordHash = HashPassword(request.NewPassword);

        user.UpdatedAt = DateTime.UtcNow;
        user.LastModifiedBy = performedBy;

        await _repo.UpdateAsync(user, ct);

        await _audit.LogAsync(
            AuditAction.UpdateProfile.ToString(),
            "User",
            $"Id={user.Id}",
            performedBy,
            ip,
            null,
            ct);

        return Map(user);
    }

    public async Task<bool> SoftDeleteAsync(
        int id,
        string performedBy,
        string? ip,
        CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct);
        if (user is null) return false;

        await _repo.SoftDeleteAsync(user, ct);

        await _audit.LogAsync(
            AuditAction.Delete.ToString(),
            "User",
            $"Id={user.Id}",
            performedBy,
            ip,
            null,
            ct);

        return true;
    }

    public async Task<PagedResult<UserDto>> ListAsync(
        UserQuery query,
        CancellationToken ct = default)
    {
        var asc = string.Equals(query.Dir, "asc", StringComparison.OrdinalIgnoreCase);
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : Math.Min(query.PageSize, 100);

        var result = await _repo.SearchAsync(
            query.Search,
            query.Role,
            page,
            pageSize,
            query.Sort,
            asc,
            ct);

        return new PagedResult<UserDto>
        {
            Items = result.Items.Select(Map).ToArray(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<UserDto?> FindByUsernameAsync(
        string username,
        CancellationToken ct = default)
    {
        var u = await _repo.GetByUsernameAsync(username, ct);
        return u is null ? null : Map(u);
    }

    public async Task<UserDto?> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken ct = default)
    {
        var u = await _repo.GetByUsernameAsync(username, ct);
        if (u is null) return null;

        return VerifyPassword(password, u.PasswordHash) ? Map(u) : null;
    }

    private static string HashPassword(string password)
    {
        using var derive = new Rfc2898DeriveBytes(password, 16, 100_000, HashAlgorithmName.SHA256);
        var salt = derive.Salt;
        var key = derive.GetBytes(32);
        return Convert.ToBase64String(salt.Concat(key).ToArray());
    }

    private static bool VerifyPassword(string password, string storedBase64)
    {
        var bytes = Convert.FromBase64String(storedBase64);
        var salt = bytes[..16];
        var key = bytes[16..];

        using var derive = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var test = derive.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(key, test);
    }

    private static UserDto Map(User u) =>
        new(
            u.Id,
            u.Username,
            u.Email,
            (UserRole)u.Role,
            u.CreatedAt,
            u.UpdatedAt
        );
}
