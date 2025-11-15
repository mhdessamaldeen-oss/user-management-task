namespace UserManagement.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public string Action { get; set; } = string.Empty; // Insert/Update/Delete
    public string EntityName { get; set; } = string.Empty; 
    public string EntityKey { get; set; } = string.Empty;  
    public string? PerformedBy { get; set; }               // username/userId
    public string? IpAddress { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public string? ChangesJson { get; set; }               
}
