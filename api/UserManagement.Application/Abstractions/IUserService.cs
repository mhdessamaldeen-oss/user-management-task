using UserManagement.Application.Models;
using UserManagement.Application.Users;

namespace UserManagement.Application.Abstractions;

public interface IUserService
{
    Task<UserDto> CreateAsync(
        CreateUserRequest request,
        string performedBy,
        string? ip,
        CancellationToken ct = default);

    Task<UserDto?> GetAsync(
        int id,
        CancellationToken ct = default);

    Task<UserDto?> UpdateAsync(
        int id,
        UpdateUserRequest request,
        string performedBy,
        string? ip,
        CancellationToken ct = default);

    /// <summary>
    /// Update current user's own profile (no role change).
    /// </summary>
    Task<UserDto?> UpdateProfileAsync(
        int userId,
        UpdateUserProfileRequest request,
        string performedBy,
        string? ip,
        CancellationToken ct = default);

    Task<bool> SoftDeleteAsync(
        int id,
        string performedBy,
        string? ip,
        CancellationToken ct = default);

    Task<PagedResult<UserDto>> ListAsync(
        UserQuery query,
        CancellationToken ct = default);

    // For login / validation:
    Task<UserDto?> FindByUsernameAsync(
        string username,
        CancellationToken ct = default);

    Task<UserDto?> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken ct = default);
}
