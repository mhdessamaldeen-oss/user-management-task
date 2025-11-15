namespace UserManagement.Application.Models;

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "An error occurred.";
    public string? Details { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}
