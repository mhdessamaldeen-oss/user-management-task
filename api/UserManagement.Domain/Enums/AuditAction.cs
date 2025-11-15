// Domain/Enums/AuditAction.cs
namespace UserManagement.Domain.Enums;

public enum AuditAction
{
    Insert = 1,
    Update = 2,
    UpdateProfile = 3,
    Delete = 4,
    // you can add more:
    LoginFailed = 10,
    RoleChanged = 11,
    View = 12,
    ViewList = 13,
    DataTableQuery = 14
}
