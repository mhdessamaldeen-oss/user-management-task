using UserManagement.Application.Users;

namespace UserManagement.Application.Abstractions;

public interface IAuditService
{
    Task LogAsync(
        string action,
        string entityName,
        string entityKey,
        string? performedBy,
        string? ip,
        string? changesJson = null,
        CancellationToken ct = default);

    Task LogChangeAsync(
        string action,
        string entityName,
        string entityKey,
        object? oldValues,
        object? newValues,
        string? performedBy,
        string? ip,
        string? description = null,
        CancellationToken ct = default);
}
