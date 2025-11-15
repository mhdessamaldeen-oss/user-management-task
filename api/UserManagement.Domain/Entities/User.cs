using UserManagement.Domain.Enums;
namespace UserManagement.Domain.Entities;


public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Role { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;
    public string? LastModifiedBy { get; set; }

}

