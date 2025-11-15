using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var u = modelBuilder.Entity<User>();
        u.HasKey(x => x.Id);
        u.Property(x => x.Username).HasMaxLength(50).IsRequired();
        u.HasIndex(x => x.Username).IsUnique();
        u.Property(x => x.Email).HasMaxLength(100);
        u.HasQueryFilter(x => !x.IsDeleted); // soft delete global filter

        var a = modelBuilder.Entity<AuditLog>();
        a.HasKey(x => x.Id);
        a.Property(x => x.Action).HasMaxLength(20).IsRequired();
        a.Property(x => x.EntityName).HasMaxLength(100).IsRequired();
        a.Property(x => x.EntityKey).HasMaxLength(100).IsRequired();
        a.Property(x => x.IpAddress).HasMaxLength(64);
    }
}
