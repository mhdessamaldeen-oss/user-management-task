using System.ComponentModel.DataAnnotations;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Users;

public record UserDto(int Id, string Username, string Email, UserRole Role, DateTime CreatedAt, DateTime? UpdatedAt);

public record CreateUserRequest(
    [Required, MinLength(3), MaxLength(50)]
    string Username,

    [Required, EmailAddress, MaxLength(100)]
    string Email,

    [Required, MinLength(8), MaxLength(100)]
    string Password,

    [Required]
    UserRole Role
);

public record UpdateUserRequest(
    [Required, MinLength(3), MaxLength(50)]
    string Username,

    [Required, EmailAddress, MaxLength(100)]
    string Email,

    [Required]
    UserRole Role,

    [MinLength(8), MaxLength(100)]
    string? NewPassword
);

public record UserQuery(
    string? Search,
    UserRole? Role,
    int Page = 1,
    int PageSize = 10,
    string? Sort = null,
    string? Dir = "asc");



public record UpdateUserProfileRequest(
    [Required, EmailAddress, MaxLength(100)]
    string Email,

    [MinLength(8), MaxLength(100)]
    string? NewPassword
);

