using UserManagement.Application.Models;
using UserManagement.Application.Users;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> UsernameExistsAsync(string username, int? excludeId = null, CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task SoftDeleteAsync(User user, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);

    Task<PagedResult<User>> SearchAsync(string? search, UserRole? role, int page, int pageSize, string? sort, bool asc, CancellationToken ct = default);
}
