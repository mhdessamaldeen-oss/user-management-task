using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Persistence.Migrations;

namespace UserManagement.Persistence;

public static class AppDbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await ctx.Database.MigrateAsync();

        // If we already have >= 15 users, consider it seeded
        if (await ctx.Users.CountAsync() >= 15)
            return;

        var password = "123456789!";
        var passwordHash = HashPassword(password);
        var now = DateTime.UtcNow;

        var seedUsers = new List<User>();

        // 5 Admins
        for (var i = 1; i <= 5; i++)
        {
            seedUsers.Add(new User
            {
                Username = $"admin{i}",
                Email = $"admin{i}@test.local",
                PasswordHash = passwordHash,
                Role = (int)UserRole.Admin,
                IsDeleted = false,
                CreatedAt = now,
                CreatedBy = "seed"
            });
        }

        // 5 normal Users
        for (var i = 1; i <= 5; i++)
        {
            seedUsers.Add(new User
            {
                Username = $"user{i}",
                Email = $"user{i}@test.local",
                PasswordHash = passwordHash,
                Role = (int)UserRole.User,
                IsDeleted = false,
                CreatedAt = now,
                CreatedBy = "seed"
            });
        }

        // 5 ReadOnly users
        for (var i = 1; i <= 5; i++)
        {
            seedUsers.Add(new User
            {
                Username = $"readonly{i}",
                Email = $"readonly{i}@test.local",
                PasswordHash = passwordHash,
                Role = (int)UserRole.ReadOnlyUser,
                IsDeleted = false,
                CreatedAt = now,
                CreatedBy = "seed"
            });
        }

        // Avoid duplicating if you already manually created some
        var existingUsernames = new HashSet<string>(
            await ctx.Users.Select(u => u.Username).ToListAsync(),
            StringComparer.OrdinalIgnoreCase);

        var newUsers = seedUsers
            .Where(u => !existingUsernames.Contains(u.Username))
            .ToList();

        if (newUsers.Count > 0)
        {
            ctx.Users.AddRange(newUsers);
            await ctx.SaveChangesAsync();
        }
    }

    // Same PBKDF2 pattern you use in UserService
    private static string HashPassword(string password)
    {
        using var derive = new Rfc2898DeriveBytes(password, 16, 100_000, HashAlgorithmName.SHA256);
        var salt = derive.Salt;
        var key = derive.GetBytes(32);
        return Convert.ToBase64String(salt.Concat(key).ToArray());
    }
}