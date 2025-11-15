using System.Text.Json;
using UserManagement.Application.Abstractions;
using UserManagement.Domain.Entities;
using UserManagement.Persistence;

namespace UserManagement.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;

    public AuditService(AppDbContext db) => _db = db;

    public async Task LogAsync(
        string action,
        string entityName,
        string entityKey,
        string? performedBy,
        string? ip,
        string? changesJson = null,
        CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityKey = entityKey,
            PerformedBy = performedBy,
            IpAddress = ip,
            ChangesJson = changesJson,
            PerformedAt = DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync(ct);
    }

    public async Task LogChangeAsync(
        string action,
        string entityName,
        string entityKey,
        object? oldValues,
        object? newValues,
        string? performedBy,
        string? ip,
        string? description = null,
        CancellationToken ct = default)
    {
        var jsonObject = new
        {
            Description = description,
            OldValues = oldValues,
            NewValues = newValues
        };

        string? changesJson = JsonSerializer.Serialize(
            jsonObject,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

        var log = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityKey = entityKey,
            PerformedBy = performedBy,
            IpAddress = ip,
            ChangesJson = changesJson,
            PerformedAt = DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync(ct);
    }
}
