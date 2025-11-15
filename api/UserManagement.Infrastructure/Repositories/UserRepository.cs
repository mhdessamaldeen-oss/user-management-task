using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Abstractions;
using UserManagement.Application.Models;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Persistence;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<bool> UsernameExistsAsync(string username, int? excludeId = null, CancellationToken ct = default)
    {
        var q = _db.Users.AsNoTracking().Where(x => x.Username == username);
        if (excludeId.HasValue) q = q.Where(x => x.Id != excludeId.Value);
        return q.AnyAsync(ct);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(User user, CancellationToken ct = default)
    {
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<User>> SearchAsync(string? search, UserRole? role, int page, int pageSize, string? sort, bool asc, CancellationToken ct = default)
    {
        var q = _db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            q = q.Where(x => x.Username.Contains(search) || x.Email.Contains(search));
        }

        if (role.HasValue) q = q.Where(x => x.Role == (int)role.Value);

        // Sorting
        q = (sort?.ToLowerInvariant()) switch
        {
            "email" => asc ? q.OrderBy(x => x.Email) : q.OrderByDescending(x => x.Email),
            "role" => asc ? q.OrderBy(x => x.Role) : q.OrderByDescending(x => x.Role),
            "created" => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt),
            "updated" => asc ? q.OrderBy(x => x.UpdatedAt) : q.OrderByDescending(x => x.UpdatedAt),
            _ => asc ? q.OrderBy(x => x.Username) : q.OrderByDescending(x => x.Username),
        };

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<User>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }
    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var u = username.Trim();
        return _db.Users
                  .AsNoTracking()
                  .FirstOrDefaultAsync(x => x.Username == u, ct);
    }

}
